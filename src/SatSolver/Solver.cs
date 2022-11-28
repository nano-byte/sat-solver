// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A Boolean Satisfiability Solver.
/// </summary>
/// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
public class Solver<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Checks whether this <paramref name="formula"/> is satisfiable.
    /// </summary>
    public bool IsSatisfiable(Formula<T> formula)
    {
        formula = formula.Simplify();
        if (formula.IsConsistent) return true;
        if (formula.ContainsEmptyClause) return false;

        var literal = ChooseLiteral(formula);
        return IsSatisfiable(formula & literal)
            || IsSatisfiable(formula & !literal);
    }

    /// <summary>
    /// Picks a <see cref="Literal{T}"/> from the <paramref name="formula"/> to assign a truth value to during backtracking.
    /// </summary>
    protected virtual Literal<T> ChooseLiteral(Formula<T> formula)
        => formula.GetLiterals().First();
}
