// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.SatSolver;

/// <summary>
/// A satisfying assignment for a <see cref="SatProblem{T}"/>. Variables that were never constrained may be missing.
/// </summary>
public sealed class Model<T>(IReadOnlyDictionary<T, bool> assignment)
    where T : IEquatable<T>
{
    /// <summary>The truth value assigned to <paramref name="value"/>, or <c>null</c> if no assignment exists.</summary>
    public bool? this[T value]
        => assignment.TryGetValue(value, out bool b) ? b : null;
}
