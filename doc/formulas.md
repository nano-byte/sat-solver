# Building formulas with operators

In addition to feeding clauses directly to a <xref:NanoByte.SatSolver.SatProblem`1>, you can assemble a Boolean expression out of <xref:NanoByte.SatSolver.Literal`1>, <xref:NanoByte.SatSolver.Clause`1> and <xref:NanoByte.SatSolver.Formula`1> objects and pass it to the solver in one go. This is useful when the constraints come from another layer (e.g. a parser or a domain model) that wants to hand the solver a complete CNF expression.

## Literals

A literal is a variable identity plus a negation flag. Any value that implements `IEquatable<T>` can be wrapped in a literal:

```csharp
var a = Literal.Of("a");
var b = Literal.Of("b");
var notA = !a;            // negation
```

Any `T` is also implicitly convertible to `Literal<T>`, so you can usually just write the underlying value:

```csharp
Literal<string> a = "a";
```

## Clauses

A <xref:NanoByte.SatSolver.Clause`1> is a disjunction of literals (at least one must be true). Use the `|` operator to combine literals into a clause:

```csharp
Clause<string> clause = a | b | !c;       // a ∨ b ∨ ¬c
```

A single literal is implicitly convertible to a unit clause:

```csharp
Clause<string> fact = a;                   // {a}
```

## Formulas

A <xref:NanoByte.SatSolver.Formula`1> is a conjunction of clauses (all must be true), i.e. a CNF expression. Use the `&` operator to combine clauses into a formula:

```csharp
Formula<string> formula = (a | b) & (!a | c) & (!c | d) & a;
```

## Handing a formula to the solver

`SatProblem<T>` accepts a `Formula<T>` via either <xref:NanoByte.SatSolver.SatProblem`1.AddFormula*> or the `+=` operator:

```csharp
var problem = new SatProblem<string>();
problem += (a | b) & (!a | c) & (!c | d) & a;

Model<string>? model = problem.Solve();
```

You do not need to call `AddVariable` first; any literal referenced in the formula will be registered on first use.

## Inspecting and simplifying a formula

`Formula<T>` is a `HashSet<Clause<T>>`, so the usual set operations work. It also exposes a few helpers:

- <xref:NanoByte.SatSolver.Formula`1.ContainsEmptyClause>: `true` if any clause is empty, in which case the formula is trivially unsatisfiable.
- <xref:NanoByte.SatSolver.Formula`1.IsConsistent>: `true` if the formula is a set of non-conflicting unit clauses (i.e. already represents a concrete assignment).
- <xref:NanoByte.SatSolver.Formula`1.GetLiterals*>: every literal referenced anywhere in the formula.
- <xref:NanoByte.SatSolver.Formula`1.GetPureLiterals*>: literals that appear with only one polarity. These can always be set true without changing satisfiability.
- <xref:NanoByte.SatSolver.Formula`1.Simplify*>: returns a new formula with unit propagation and pure-literal elimination applied to a fixed point.

```csharp
Formula<string> simplified = formula.Simplify();
if (simplified.ContainsEmptyClause)
{
    // Unsatisfiable without even invoking the solver.
}
```

Simplification is purely a preprocessing convenience; the CDCL engine performs unit propagation internally during the search.
