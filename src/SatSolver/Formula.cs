// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A Boolean Formula. Consists of a set of <see cref="Clause{T}"/>s which all must be true.
/// </summary>
/// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
public class Formula<T> : HashSet<Clause<T>>, IEquatable<Formula<T>>
    where T : IEquatable<T>
{

    /// <summary>
    /// Creates an empty Formula.
    /// </summary>
    public Formula()
    {}

    /// <summary>
    /// Creates a Formula consisting the specified <paramref name="clauses"/>.
    /// </summary>
    public Formula(IEnumerable<Clause<T>> clauses)
        : base(clauses)
    {}

    /// <summary>
    /// Creates a <see cref="Formula{T}"/> consisting of all Clauses from an existing Formula plus an additional <see cref="Clause{T}"/>.
    /// </summary>
    /// <param name="formula">The existing Formula.</param>
    /// <param name="clause">The additional Clause.</param>
    public static Formula<T> operator &(Formula<T> formula, Clause<T> clause)
        => new(formula) {clause};

    /// <summary>
    /// Creates a <see cref="Formula{T}"/> consisting of all Clauses from an existing Formula plus an additional <see cref="Clause{T}"/>.
    /// </summary>
    /// <param name="clause">The additional Clause.</param>
    /// <param name="formula">The existing Formula.</param>
    public static Formula<T> operator &(Clause<T> clause, Formula<T> formula)
        => new(formula) {clause};

    /// <summary>
    /// Indicates whether this Formula contains any empty Clauses and is therefore unsatisfiable.
    /// </summary>
    public bool ContainsEmptyClause
        => this.Any(clause => clause.IsEmpty);

    /// <summary>
    /// Indicates whether this Formula is a consistent set of Literals, i.e. consists only non-conflicting Unit Clauses.
    /// </summary>
    public bool IsConsistent
    {
        get
        {
            var assigned = new Dictionary<T, bool>();
            foreach (var clause in this)
            {
                if (!clause.IsUnit) return false; // Only unit clauses allowed
                var literal = clause.First();
                if (assigned.TryGetValue(literal.Value, out bool existing))
                {
                    if (existing != !literal.Negated) return false; // Conflict detected
                }
                else
                {
                    assigned[literal.Value] = !literal.Negated; // Store literal's value
                }
            }
            return true;
        }
    }

    private struct Polarity
    {
        public bool Positive, Negative;
    }

    /// <summary>
    /// Returns a set of all <see cref="Literal{T}"/>s referenced in the Formula that are pure, i.e. do not occur both negated and non-negated.
    /// </summary>
    public IEnumerable<Literal<T>> GetPureLiterals()
    {
        var map = new Dictionary<T, Polarity>();

        foreach (var clause in this)
        foreach (var lit in clause)
        {
            if (!map.TryGetValue(lit.Value, out var entry))
                entry = new();

            if (lit.Negated) entry.Negative = true;
            else entry.Positive = true;

            map[lit.Value] = entry;
        }

        foreach (var entry in map)
        {
            if (entry.Value.Positive ^ entry.Value.Negative) // exactly one polarity
                yield return new(entry.Key, entry.Value.Negative);
        }
    }

    /// <summary>
    /// Returns a set of all <see cref="Literal{T}"/>s referenced in the Formula.
    /// </summary>
    public IEnumerable<Literal<T>> GetLiterals()
        => this.SelectMany(clause => clause).Distinct();

    /// <summary>
    /// Returns a simplified copy of the Formula. This applies Unit propagation and Pure Literal elimination.
    /// </summary>
    public Formula<T> Simplify()
    {
        var current = this;
        while (true)
        {
            var simplified= current.PropagateUnits().EliminatePureLiterals();
            if (simplified.Count == current.Count && simplified.All(current.Contains))
                return current;
            current = simplified;
        }
    }

    /// <summary>
    /// Returns a copy of the Formula simplified by propagating all Unit Clauses.
    /// </summary>
    internal Formula<T> PropagateUnits()
        => new(
            this.Where(clause => clause.IsUnit).Select(x => x.Single())
                .Aggregate(this.AsEnumerable(), (clauses, literal)
                     => clauses.Where(clause => clause.IsUnit || !clause.Contains(literal))
                               .Select(clause => clause.Without(!literal))));

    /// <summary>
    /// Returns a copy of the Formula simplified by remove all Clauses that contain Pure Literals.
    /// </summary>
    internal Formula<T> EliminatePureLiterals()
    {
        var pureSet = new HashSet<Literal<T>>(GetPureLiterals());
        return pureSet.Count == 0
            ? this
            : new(this.Where(clause => !clause.Any(pureSet.Contains)));
    }

    public override string ToString()
        => string.Join(" & ", this.Select(clause => clause.ToString()).ToArray());

    public bool Equals(Formula<T>? other)
        => other != null && Count == other.Count && other.All(Contains);

    public override bool Equals(object? obj)
        => obj is Formula<T> other && Equals(other);

    public override int GetHashCode()
        => this.Aggregate(397, HashCode.Combine);
}
