// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
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
        => IsSatisfiableInner(formula, visited: new());

    private bool IsSatisfiableInner(Formula<T> formula, HashSet<Formula<T>> visited)
    {
        formula = formula.Simplify();
        if (formula.IsConsistent) return true;
        if (formula.ContainsEmptyClause) return false;

        if (!visited.Add(formula)) return false;

        while (true)
        {
            var unitClause = formula.FirstOrDefault(c => c.IsUnit);
            if (unitClause == null) break;

            var unitLiteral = unitClause.First();
            formula = (formula & unitLiteral).Simplify();

            if (formula.ContainsEmptyClause) return false;
            if (formula.IsConsistent) return true;
        }

        var allLiterals = formula.SelectMany(c => c).ToList();
        foreach (var literal in allLiterals.Distinct())
        {
            if (literal.IsPure(allLiterals))
                formula = (formula & literal).Simplify();
        }

        if (formula.IsConsistent) return true;
        if (formula.ContainsEmptyClause) return false;

        var chosen = ChooseLiteral(formula);

        return IsSatisfiableInner(formula & chosen, visited)
            || IsSatisfiableInner(formula & !chosen, visited);
    }

    /// <summary>
    /// Picks a <see cref="Literal{T}"/> from the <paramref name="formula"/> to assign a truth value to during backtracking.
    /// </summary>
    protected virtual Literal<T> ChooseLiteral(Formula<T> formula)
        => formula
          .OrderBy(x => x.Count).First() // Heuristic
          .First();
}
