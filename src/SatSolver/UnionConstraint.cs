// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A disjunctive clause (at least one literal must be true) implemented with the two-watched-literal scheme.
/// </summary>
/// <remarks>At all times two of the clause's literals are watched; the constraint is only re-evaluated when one of those two has its variable assigned in a way that makes the watched literal false.</remarks>
internal sealed class UnionConstraint<T> : IConstraint<T>
    where T : IEquatable<T>
{
    private readonly Lit<T>[] _literals;

    /// <summary>
    /// Creates a new constraint over the given literals.
    /// </summary>
    /// <remarks>Caller is responsible for registering watches via <see cref="RegisterWatches"/> after the engine has been set up.</remarks>
    public UnionConstraint(IReadOnlyList<Lit<T>> literals)
    {
        if (literals.Count < 2) throw new ArgumentException("UnionConstraint requires at least two literals; use Enqueue for unit facts.", nameof(literals));
        _literals = literals.ToArray();
    }

    /// <summary>
    /// Registers the two initial watches on this constraint's first two literals. Must be called once before propagation.
    /// </summary>
    public void RegisterWatches()
    {
        Watch(_literals[0]);
        Watch(_literals[1]);
    }

    /// <inheritdoc/>
    public bool Propagate(SatEngine<T> engine, Lit<T> becameFalse)
    {
        // Make the literal that just became false the second watch (index 1)
        if (_literals[0].Equals(becameFalse))
            (_literals[0], _literals[1]) = (_literals[1], _literals[0]);

        // If the other watch is already true the clause is satisfied; keep watching
        if (_literals[0].Value == true)
        {
            Watch(becameFalse); // re-register; the engine removed us before calling
            return true;
        }

        // Look for a non-false replacement among the unwatched literals (indices 2..N)
        for (int i = 2; i < _literals.Length; i++)
        {
            if (_literals[i].Value != false)
            {
                _literals[1] = _literals[i];
                _literals[i] = becameFalse;
                Watch(_literals[1]);
                return true;
            }
        }

        // No replacement available: index 0 is the last hope
        Watch(becameFalse); // re-register so we still receive notifications after backtracking
        if (_literals[0].Value == null)
        {
            // Unit propagation: force the remaining unassigned watch true
            return engine.Enqueue(_literals[0], reason: this);
        }

        // _literals[0].Value == false → conflict
        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<Lit<T>> ConflictReason()
    {
        // The clause is violated when every literal is false, so the conflict cause is the negation of every literal.
        foreach (var lit in _literals) yield return lit.Negate();
    }

    /// <inheritdoc/>
    public IEnumerable<Lit<T>> PropagationReason(Lit<T> forced)
        => from lit in _literals where !lit.Equals(forced) select lit.Negate();

    private void Watch(Lit<T> literal)
    {
        var watchList = literal.Negated ? literal.Variable.WatchedAsTrue : literal.Variable.WatchedAsFalse;
        watchList.Add(this);
    }

    public override string ToString()
        => "(" + string.Join(" | ", _literals.Select(l => l.ToString())) + ")";
}
