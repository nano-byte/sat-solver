// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.SatSolver;

/// <summary>
/// A Boolean variable inside the CDCL engine.
/// </summary>
internal sealed class Variable<T>(int id, T userData)
    where T : IEquatable<T>
{
    public int Id { get; } = id;
    public T UserData { get; } = userData;

    /// <summary>
    /// The current value: <c>true</c>, <c>false</c>, or <c>null</c> for unassigned.
    /// </summary>
    public bool? Value;

    /// <summary>
    /// The constraint that propagated this assignment, or <c>null</c> for decisions / external facts.
    /// </summary>
    public IConstraint<T>? Reason;

    /// <summary>
    /// The decision level at which this variable was assigned. <c>0</c> means the assignment is unconditional (a fact or a forced top-level consequence).
    /// </summary>
    public int Level;

    /// <summary>
    /// Constraints that need to be re-checked when this variable becomes <c>true</c> (i.e. when a literal of the form <c>!v</c> in some clause becomes false).
    /// </summary>
    public readonly List<IConstraint<T>> WatchedAsTrue = [];

    /// <summary>
    /// Constraints that need to be re-checked when this variable becomes <c>false</c> (i.e. when a literal of the form <c>v</c> in some clause becomes false).
    /// </summary>
    public readonly List<IConstraint<T>> WatchedAsFalse = [];

    public override string ToString()
        => Value switch
        {
            true => $"{UserData}=T@{Level}",
            false => $"{UserData}=F@{Level}",
            null => $"{UserData}=?"
        };
}
