// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;

namespace NanoByte.SatSolver;

/// <summary>
/// A constraint participating in CDCL propagation.
/// </summary>
internal interface IConstraint<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Called when a literal of this constraint has just become <c>false</c> via an assignment.
    /// </summary>
    /// <param name="engine">The engine running the propagation.</param>
    /// <param name="becameFalse">The literal in this constraint that just became <c>false</c>.</param>
    /// <returns><c>false</c> if a conflict was detected (constraint is fully violated); <c>true</c> otherwise.</returns>
    /// <remarks>The implementation should propagate consequences (forcing other literals via <see cref="SatEngine{T}.Enqueue"/>) and re-watch literals as needed.</remarks>
    bool Propagate(SatEngine<T> engine, Lit<T> becameFalse);

    /// <summary>
    /// Returns the set of literals whose simultaneous truth caused this constraint to be violated.
    /// </summary>
    /// <remarks>Used by conflict analysis when this constraint is the conflicting clause.</remarks>
    IEnumerable<Lit<T>> ConflictReason();

    /// <summary>
    /// Returns the set of literals whose simultaneous truth forced <paramref name="forced"/> to be assigned.
    /// </summary>
    /// <remarks>Used by conflict analysis to walk the implication graph.</remarks>
    IEnumerable<Lit<T>> PropagationReason(Lit<T> forced);
}
