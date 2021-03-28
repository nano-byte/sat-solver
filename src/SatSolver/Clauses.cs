// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver
{
    /// <summary>
    /// Static factory methods for <see cref="Clause{T}"/>.
    /// </summary>
    public static class Clauses
    {
        /// <summary>
        /// Creates Clauses that together prevent more than one of the specified <paramref name="literals"/> from being true.
        /// </summary>
        /// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
        public static IEnumerable<Clause<T>> AtMostOne<T>(params Literal<T>[] literals)
            where T : IEquatable<T>
        {
            for (int i = 0; i < literals.Length; i++)
            for (int j = i + 1; j < literals.Length; j++)
                yield return !literals[i] | !literals[j];
        }

        /// <summary>
        /// Creates Clauses that together prevent more than one of the specified <paramref name="literals"/> from being true.
        /// </summary>
        /// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
        public static IEnumerable<Clause<T>> AtMostOne<T>(IEnumerable<Literal<T>> literals)
            where T : IEquatable<T>
            => AtMostOne(literals.ToArray());
    }
}
