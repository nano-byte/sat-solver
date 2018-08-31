// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver
{
    /// <summary>
    /// A Boolean Clause. Consists of a set of <see cref="Literal{T}"/>s of which at least one must be true.
    /// </summary>
    /// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
    public class Clause<T> : HashSet<Literal<T>>, IEquatable<Clause<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Creates an empty Clause.
        /// </summary>
        public Clause()
        {}

        /// <summary>
        /// Creates a Clause consisting the specified <paramref name="literals"/>.
        /// </summary>
        public Clause(IEnumerable<Literal<T>> literals) : base(literals)
        {}

        /// <summary>
        /// Creates a <see cref="Clause{T}"/> consisting of all Literals from an existing Clause plus an additional <see cref="Literal{T}"/>.
        /// </summary>
        /// <param name="clause">The existing Clause.</param>
        /// <param name="literal">The additional Literal.</param>
        public static Clause<T> operator |(Clause<T> clause, Literal<T> literal)
            => new Clause<T>(clause) {literal};

        /// <summary>
        /// Creates a <see cref="Clause{T}"/> consisting of all Literals from an existing Clause plus an additional <see cref="Literal{T}"/>.
        /// </summary>
        /// <param name="literal">The additional Literal.</param>
        /// <param name="clause">The existing Clause.</param>
        public static Clause<T> operator |(Literal<T> literal, Clause<T> clause)
            => new Clause<T>(clause) {literal};

        /// <summary>
        /// Creates a <see cref="Formula{T}"/> consisting of two <see cref="Clause{T}"/>s.
        /// </summary>
        /// <param name="clause1">The first Clause.</param>
        /// <param name="clause2">The second Clause.</param>
        public static Formula<T> operator &(Clause<T> clause1, Clause<T> clause2)
            => new Formula<T> {clause1, clause2};

        /// <summary>
        /// Returns a negated copy of this Clause without the specified <paramref name="literal"/>.
        /// </summary>
        public Clause<T> Without(Literal<T> literal)
            => new Clause<T>(this.Where(x => x != literal));

        /// <summary>
        /// Indicates whether this Clause is empty and therefore unsatisfiable.
        /// </summary>
        public bool IsEmpty
            => Count == 0;

        /// <summary>
        /// Indicates whether this is a Unit Clause, i.e. contains exactly one Literal.
        /// </summary>
        public bool IsUnit
            => Count == 1;

        public override string ToString()
            => "(" + string.Join("|", this.Select(literal => literal.ToString()).ToArray()) + ")";

        public bool Equals(Clause<T> other)
            => other != null && Count == other.Count && other.All(Contains);

        public override bool Equals(object obj)
            => obj is Clause<T> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 397;
                foreach (var literal in this)
                    result = result ^ literal.GetHashCode();
                return result;
            }
        }
    }
}
