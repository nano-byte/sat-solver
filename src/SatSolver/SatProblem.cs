// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A mutable SAT problem builder.
/// </summary>
/// <typeparam name="T">The user-data type used to identify variables. Each distinct value gets one Boolean variable.</typeparam>
/// <remarks>Allocate variables for the values you care about, add clauses and other constraints, then call <see cref="Solve"/> to obtain a satisfying assignment (or <c>null</c> if unsatisfiable).</remarks>
public sealed class SatProblem<T>
    where T : IEquatable<T>
{
    private readonly SatEngine<T> _engine = new();
    private readonly Dictionary<T, Variable<T>> _byUserData = [];
    private bool _solved;

    /// <summary>
    /// Returns the literal for <paramref name="value"/>, allocating a new variable on first use.
    /// </summary>
    public Literal<T> AddVariable(T value)
    {
        if (!_byUserData.TryGetValue(value, out var v))
        {
            v = _engine.AddVariable(value);
            _byUserData[value] = v;
        }
        return new(value, negated: false);
    }

    /// <summary>
    /// Adds a clause requiring at least one of <paramref name="literals"/> to be true.
    /// An empty clause makes the problem unsatisfiable. A single literal becomes an unconditional fact.
    /// </summary>
    public void AddClause(IEnumerable<Literal<T>> literals)
    {
        var deduped = Dedupe(literals, out bool tautology);
        if (tautology) return; // contains both v and !v → trivially satisfied
        AddInternalClause(deduped);
    }

    /// <inheritdoc cref="AddClause(IEnumerable{Literal{T}})"/>
    public void AddClause(params Literal<T>[] literals) => AddClause((IEnumerable<Literal<T>>)literals);

    /// <summary>
    /// Adds all clauses from <paramref name="formula"/> to the problem.
    /// </summary>
    public void AddFormula(Formula<T> formula)
    {
        foreach (var clause in formula)
            AddClause(clause);
    }

    /// <summary>
    /// Adds a clause to the problem. Enables <c>problem += literal1 | literal2</c> syntax.
    /// </summary>
    public static SatProblem<T> operator +(SatProblem<T> problem, Clause<T> clause)
    {
        problem.AddClause(clause);
        return problem;
    }

    /// <summary>
    /// Adds all clauses from a formula to the problem. Enables <c>problem += (a | b) &amp; (!a | c)</c> syntax.
    /// </summary>
    public static SatProblem<T> operator +(SatProblem<T> problem, Formula<T> formula)
    {
        problem.AddFormula(formula);
        return problem;
    }

    /// <summary>
    /// Adds the implication <paramref name="antecedent"/> → at-least-one-of(<paramref name="consequents"/>).
    /// Equivalent to a clause: <c>!antecedent ∨ c1 ∨ c2 ∨ ...</c>.
    /// </summary>
    public void Implies(Literal<T> antecedent, params Literal<T>[] consequents)
    {
        var clause = new List<Literal<T>>(consequents.Length + 1) {antecedent.Negate()};
        clause.AddRange(consequents);
        AddClause(clause);
    }

    /// <summary>
    /// Adds an at-most-one constraint over the given literals. Returns the constraint object so callers (e.g., deciders) can query it.
    /// </summary>
    public AtMostOneConstraint<T> AtMostOne(IEnumerable<Literal<T>> literals)
    {
        var internalLits = literals.Select(ToInternal).ToList();
        var constraint = new AtMostOneConstraint<T>(internalLits);
        constraint.RegisterWatches();
        return constraint;
    }

    /// <inheritdoc cref="AtMostOne(IEnumerable{Literal{T}})"/>
    public AtMostOneConstraint<T> AtMostOne(params Literal<T>[] literals) => AtMostOne((IEnumerable<Literal<T>>)literals);

    /// <summary>
    /// Returns the current truth value of <paramref name="literal"/> in the engine, or <c>null</c> if it has not yet been assigned. Useful from inside a decider callback.
    /// </summary>
    public bool? GetValue(Literal<T> literal)
    {
        if (!_byUserData.TryGetValue(literal.Value, out var v) || v.Value is null) return null;
        return v.Value.Value ^ literal.Negated;
    }

    /// <summary>
    /// Solves the problem.
    /// </summary>
    /// <param name="decider">
    /// Optional callback to choose the next literal to branch on. Returning <c>null</c> means "no preference; set remaining variables to false".
    /// The callback receives no arguments; it should inspect external state (e.g., a dependency graph) plus any held <see cref="AtMostOneConstraint{T}"/> references to decide.
    /// </param>
    /// <returns>A model with the satisfying assignment, or <c>null</c> if the problem is unsatisfiable.</returns>
    public Model<T>? Solve(Func<Literal<T>?>? decider = null)
    {
        if (_solved) throw new InvalidOperationException("This SatProblem has already been solved. Build a new instance for another solve.");
        _solved = true;

        Func<Lit<T>?>? internalDecider = decider == null ? null : () =>
        {
            var pub = decider();
            return pub.HasValue ? ToInternal(pub.Value) : null;
        };

        if (!_engine.Solve(internalDecider)) return null;

        var assignment = new Dictionary<T, bool>();
        foreach (var kv in _byUserData)
        {
            if (kv.Value.Value is { } v) assignment[kv.Key] = v;
        }
        return new(assignment);
    }

    private void AddInternalClause(List<Literal<T>> literals)
    {
        if (literals.Count == 0)
        {
            _engine.MarkUnsatisfiable();
            return;
        }
        if (literals.Count == 1)
        {
            if (!_engine.Enqueue(ToInternal(literals[0]), reason: null))
                _engine.MarkUnsatisfiable();
            return;
        }
        var internalLits = literals.Select(ToInternal).ToList();
        var clause = new UnionConstraint<T>(internalLits);
        clause.RegisterWatches();
    }

    private Lit<T> ToInternal(Literal<T> lit)
    {
        if (!_byUserData.TryGetValue(lit.Value, out var v))
        {
            v = _engine.AddVariable(lit.Value);
            _byUserData[lit.Value] = v;
        }
        return new(v, lit.Negated);
    }

    private static List<Literal<T>> Dedupe(IEnumerable<Literal<T>> input, out bool tautology)
    {
        var seenPos = new HashSet<T>();
        var seenNeg = new HashSet<T>();
        var output = new List<Literal<T>>();
        tautology = false;
        foreach (var lit in input)
        {
            if (lit.Negated)
            {
                if (seenPos.Contains(lit.Value)) { tautology = true; return output; }
                if (seenNeg.Add(lit.Value)) output.Add(lit);
            }
            else
            {
                if (seenNeg.Contains(lit.Value)) { tautology = true; return output; }
                if (seenPos.Add(lit.Value)) output.Add(lit);
            }
        }
        return output;
    }

}
