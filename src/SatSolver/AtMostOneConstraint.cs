// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A constraint that no more than one of its literals may be true.
/// </summary>
public sealed class AtMostOneConstraint<T>
    where T : IEquatable<T>
{
    private readonly Lit<T>[] _literals;

    internal AtMostOneConstraint(IReadOnlyList<Lit<T>> literals)
    {
        _literals = literals.ToArray();
    }

    /// <summary>The literal that has been assigned <c>true</c>, or <c>null</c> if no such assignment has been made yet. Recomputed from current state, so it reflects the engine after backtracking.</summary>
    public Literal<T>? Selected
    {
        get
        {
            var s = SelectedInternal();
            return s?.ToPublic();
        }
    }

    private Lit<T>? SelectedInternal()
    {
        foreach (var lit in _literals)
        {
            if (lit.Value == true) return lit;
        }
        return null;
    }

    /// <summary>
    /// Returns the first undecided literal in declaration order, or <c>null</c> if none remain or one has already been selected.
    /// Useful for deciders that want to bias selection toward earlier (preferred) candidates.
    /// </summary>
    public Literal<T>? BestUndecided()
    {
        if (SelectedInternal() != null) return null;
        foreach (var lit in _literals)
        {
            if (lit.Value == null) return lit.ToPublic();
        }
        return null;
    }

    internal Lit<T>? BestUndecidedInternal()
    {
        if (SelectedInternal() != null) return null;
        foreach (var lit in _literals)
        {
            if (lit.Value == null) return lit;
        }
        return null;
    }

    /// <summary>Registers a per-literal adapter on each variable's watch list so the engine notifies us whenever any literal becomes <c>true</c>.</summary>
    internal void RegisterWatches()
    {
        foreach (var lit in _literals)
        {
            // To be notified when this literal becomes true: positive literal → WatchedAsTrue, negative literal → WatchedAsFalse.
            var watchList = lit.Negated ? lit.Variable.WatchedAsFalse : lit.Variable.WatchedAsTrue;
            watchList.Add(new Adapter(this, lit));
        }
    }

    /// <summary>Adapter that hooks into the engine's per-variable watch list. Each adapter is anchored to one literal of the AtMostOne; when its hook fires, that specific literal has just become true.</summary>
    private sealed class Adapter(AtMostOneConstraint<T> parent, Lit<T> literal) : IConstraint<T>
    {
        public bool Propagate(SatEngine<T> engine, Lit<T> becameFalse)
        {
            // Engine removed us from the watch list before calling; re-register so we keep firing across backtracking.
            (literal.Negated ? literal.Variable.WatchedAsFalse : literal.Variable.WatchedAsTrue).Add(this);

            return parent.OnLiteralBecameTrue(engine, literal, this);
        }

        public IEnumerable<Lit<T>> ConflictReason()
            => parent.ConflictReasonFor(literal);

        public IEnumerable<Lit<T>> PropagationReason(Lit<T> forced)
            => parent.PropagationReasonFor();
    }

    private bool OnLiteralBecameTrue(SatEngine<T> engine, Lit<T> trigger, IConstraint<T> reason)
    {
        // If another literal is already true, we have a violation.
        foreach (var lit in _literals)
        {
            if (lit.Equals(trigger)) continue;
            if (lit.Value == true) return false;
        }

        // Force every other undecided literal false.
        foreach (var lit in _literals)
        {
            if (lit.Equals(trigger)) continue;
            if (lit.Value == null)
            {
                if (!engine.Enqueue(lit.Negate(), reason)) return false;
            }
        }
        return true;
    }

    private IEnumerable<Lit<T>> ConflictReasonFor(Lit<T> trigger)
    {
        // Conflict means trigger is true AND some other literal in the constraint is also true. Both literals' truth caused the violation.
        yield return trigger;
        foreach (var lit in _literals)
        {
            if (lit.Equals(trigger)) continue;
            if (lit.Value == true) yield return lit;
        }
    }

    private IEnumerable<Lit<T>> PropagationReasonFor()
    {
        // 'forced' is the literal we forced false (i.e. forced.Negate() is the original AtMostOne literal). The reason it was forced is that the unique currently-true literal in the constraint is true.
        foreach (var lit in _literals)
        {
            if (lit.Value == true) yield return lit;
        }
    }
}
