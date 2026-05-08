// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.SatSolver;

/// <summary>
/// A Boolean Satisfiability Solver.
/// </summary>
/// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
[Obsolete($"Use {nameof(SatProblem<>)} instead.")]
public class Solver<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Checks whether <paramref name="formula"/> is satisfiable.
    /// </summary>
    public bool IsSatisfiable(Formula<T> formula)
    {
        var problem = new SatProblem<T>();
        foreach (var clause in formula)
            problem.AddClause(clause);
        return problem.Solve() != null;
    }
}
