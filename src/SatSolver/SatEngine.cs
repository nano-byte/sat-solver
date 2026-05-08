// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.SatSolver;

/// <summary>
/// CDCL SAT engine. Maintains the variable assignments, the trail, the propagation queue, and the constraint database.
/// </summary>
internal sealed class SatEngine<T>
    where T : IEquatable<T>
{
    private readonly List<Variable<T>> _variables = [];

    /// <summary>Order in which literals have been assigned. Most recent at the end.</summary>
    private readonly List<Lit<T>> _trail = [];

    /// <summary>For each decision level &gt;= 1, the trail index just before that level's decision was placed. Level 0 has no entry; its entries are at the start of the trail.</summary>
    private readonly List<int> _trailLimits = [];

    /// <summary>Queue of literals whose consequences have not yet been propagated.</summary>
    private readonly Queue<Lit<T>> _propagationQueue = new();

    private bool _topLevelConflict;

    public Variable<T> AddVariable(T data)
    {
        var v = new Variable<T>(_variables.Count, data);
        _variables.Add(v);
        return v;
    }

    /// <summary>
    /// Marks the problem as unconditionally unsatisfiable (e.g., an empty clause was added).
    /// </summary>
    public void MarkUnsatisfiable() => _topLevelConflict = true;

    /// <summary>
    /// Asserts <paramref name="literal"/> is true. If <paramref name="reason"/> is <c>null</c>, this is treated as a top-level fact (level 0).
    /// </summary>
    /// <returns><c>false</c> if the assertion immediately conflicts with an existing assignment.</returns>
    public bool Enqueue(Lit<T> literal, IConstraint<T>? reason)
    {
        switch (literal.Value)
        {
            case true: return true;
            case false: return false; // conflict: variable already has the opposite value
            default:
                literal.Variable.Value = !literal.Negated;
                literal.Variable.Reason = reason;
                literal.Variable.Level = _trailLimits.Count;
                _trail.Add(literal);
                _propagationQueue.Enqueue(literal);
                return true;
        }
    }

    /// <summary>
    /// Drains the propagation queue, calling each watched constraint. Returns the conflicting constraint if a conflict is detected, or <c>null</c> on success.
    /// </summary>
    private IConstraint<T>? Propagate()
    {
        while (_propagationQueue.Count > 0)
        {
            var lit = _propagationQueue.Dequeue();
            // Watch lists keyed by "the literal's variable just got its value": if lit.Negated == false the variable became true, so we trigger watchers in WatchedAsTrue (those have !var in their clause). If lit.Negated == true the variable became false, so we trigger WatchedAsFalse.
            var (watchList, becameFalseInClause) = lit.Negated
                ? (lit.Variable.WatchedAsFalse, new Lit<T>(lit.Variable, negated: false))
                : (lit.Variable.WatchedAsTrue, new Lit<T>(lit.Variable, negated: true));

            // Take a snapshot and clear the list so propagators can re-add themselves (re-watching). This matches MiniSat-style: each constraint either re-adds itself or moves to a different watch.
            var pending = watchList.ToArray();
            watchList.Clear();

            for (int i = 0; i < pending.Length; i++)
            {
                if (!pending[i].Propagate(this, becameFalseInClause))
                {
                    // Conflict. Re-add remaining unprocessed pending so state is consistent on backtrack.
                    for (int j = i + 1; j < pending.Length; j++) watchList.Add(pending[j]);
                    _propagationQueue.Clear();
                    return pending[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Walks the implication graph backwards from <paramref name="conflict"/> to derive a learned clause via 1-UIP. Determines the level to backtrack to.
    /// </summary>
    private (List<Lit<T>> learned, int backtrackLevel) Analyze(IConstraint<T> conflict)
    {
        // 'seen' marks variables we've already pulled into the cut. The frontier is the set of "currently true" literals contributing to the conflict; we resolve those at the current decision level until exactly one remains.
        var seen = new HashSet<int>();
        var learned = new List<Lit<T>>();
        int currentLevelCount = 0;
        int backtrackLevel = 0;

        IEnumerable<Lit<T>> reasonLiterals = conflict.ConflictReason();
        int trailIndex = _trail.Count - 1;
        Lit<T>? uip = null;

        while (true)
        {
            foreach (var rLit in reasonLiterals)
            {
                // 'rLit' is currently TRUE and contributed to the conflict. We add !rLit to the learned clause.
                var v = rLit.Variable;
                if (!seen.Add(v.Id)) continue;
                if (v.Level == 0) continue; // unconditional facts don't go into the learned clause

                if (v.Level == _trailLimits.Count)
                    currentLevelCount++;
                else
                {
                    learned.Add(rLit.Negate());
                    if (v.Level > backtrackLevel) backtrackLevel = v.Level;
                }
            }

            // Walk back through the trail to find the next "seen" variable at the current level.
            while (trailIndex >= 0 && !seen.Contains(_trail[trailIndex].Variable.Id))
                trailIndex--;

            if (trailIndex < 0) break;

            var p = _trail[trailIndex];
            trailIndex--;
            currentLevelCount--;

            if (currentLevelCount == 0)
            {
                uip = p;
                break;
            }

            // Resolve through p's reason
            seen.Remove(p.Variable.Id);
            if (p.Variable.Reason == null)
                throw new InvalidOperationException($"Conflict analysis reached decision variable {p.Variable} before the UIP was found (currentLevelCount={currentLevelCount + 1}). The implication graph has a cycle, which indicates a bug in constraint propagation.");
            reasonLiterals = p.Variable.Reason.PropagationReason(p);
        }

        if (uip.HasValue)
            learned.Add(uip.Value.Negate());

        return (learned, backtrackLevel);
    }

    /// <summary>Reverts assignments back to the start of <paramref name="level"/>. If <paramref name="level"/> is 0, undoes everything except level-0 facts.</summary>
    private void BacktrackTo(int level)
    {
        if (level >= _trailLimits.Count) return;

        int newTrailLength = _trailLimits[level];

        for (int i = _trail.Count - 1; i >= newTrailLength; i--)
        {
            var lit = _trail[i];
            lit.Variable.Value = null;
            lit.Variable.Reason = null;
            lit.Variable.Level = 0;
        }
        _trail.RemoveRange(newTrailLength, _trail.Count - newTrailLength);
        _trailLimits.RemoveRange(level, _trailLimits.Count - level);
        _propagationQueue.Clear();
    }

    /// <summary>
    /// Records a new decision level and asserts <paramref name="decision"/> at that level.
    /// </summary>
    private void Decide(Lit<T> decision)
    {
        _trailLimits.Add(_trail.Count);
        Enqueue(decision, reason: null);
    }

    /// <summary>
    /// Runs the main CDCL loop. Returns <c>true</c> iff a satisfying assignment was found. The decider, when supplied, is asked to suggest the next literal to branch on; returning <c>null</c> means "no preference, set remaining to false".
    /// </summary>
    public bool Solve(Func<Lit<T>?>? decider)
    {
        if (_topLevelConflict) return false;

        // Initial top-level propagation (any unit facts already enqueued).
        if (Propagate() != null) { _topLevelConflict = true; return false; }

        while (true)
        {
            // Pick a literal to branch on
            Lit<T>? choice = decider?.Invoke();
            if (choice == null)
            {
                // Fallback: assign every remaining variable false (deterministic completion).
                Variable<T>? next = null;
                foreach (var v in _variables)
                {
                    if (v.Value == null) { next = v; break; }
                }
                if (next == null) return true; // SAT: every variable assigned
                choice = new(next, negated: true);
            }
            else if (choice.Value.Value != null)
            {
                // Decider returned a literal that is already assigned. Skip and ask again.
                continue;
            }

            Decide(choice.Value);

            // Propagate; if conflict, learn and backtrack
            while (true)
            {
                var conflict = Propagate();
                if (conflict == null) break;

                if (_trailLimits.Count == 0)
                {
                    _topLevelConflict = true;
                    return false;
                }

                var (learned, btLevel) = Analyze(conflict);
                BacktrackTo(btLevel);

                if (learned.Count == 0)
                {
                    // No learned clause means the UIP itself was at level 0 (unconditional), so we can't recover.
                    _topLevelConflict = true;
                    return false;
                }
                if (learned.Count == 1)
                {
                    // Unit learned clause: assert the sole literal as a level-0 fact (reason: null).
                    // reason: null is correct here — BacktrackTo(0) was just called so DecisionLevel == 0,
                    // and level-0 facts are unconditionally skipped in Analyze, so a richer reason would
                    // never be consulted. UnionConstraint also requires ≥ 2 literals and cannot be used.
                    if (!Enqueue(learned[0], reason: null))
                    {
                        _topLevelConflict = true;
                        return false;
                    }
                }
                else
                {
                    // Place the UIP literal first so that becomes one of the watched literals; reorder so the second-watch is the highest-level remaining literal (which will be the next to flip on backtracking).
                    var uipLit = learned[learned.Count - 1];
                    learned.RemoveAt(learned.Count - 1);
                    learned.Insert(0, uipLit);

                    // Find the literal with the highest decision level among learned[1..] and put it at index 1.
                    int bestIdx = 1;
                    int bestLevel = learned.Count > 1 ? learned[1].Variable.Level : -1;
                    for (int i = 2; i < learned.Count; i++)
                    {
                        if (learned[i].Variable.Level > bestLevel)
                        {
                            bestLevel = learned[i].Variable.Level;
                            bestIdx = i;
                        }
                    }
                    if (bestIdx != 1) (learned[1], learned[bestIdx]) = (learned[bestIdx], learned[1]);

                    var learnedClause = new UnionConstraint<T>(learned);
                    learnedClause.RegisterWatches();
                    if (!Enqueue(learned[0], reason: learnedClause))
                    {
                        _topLevelConflict = true;
                        return false;
                    }
                }
            }
        }
    }

    public IReadOnlyList<Variable<T>> Variables => _variables;
}
