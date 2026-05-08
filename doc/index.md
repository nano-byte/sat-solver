---
title: Home
---

# NanoByte SAT Solver

NanoByte SAT Solver is a CDCL Boolean Satisfiability Solver for .NET.

## Usage

Add a reference to the [NanoByte.SatSolver](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package to your project. It is available for .NET Framework 2.0+ and .NET Standard 1.0+.

You need to choose the underlying type to use for [Literals](xref:NanoByte.SatSolver.Literal`1) in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. Create an instance of <xref:NanoByte.SatSolver.SatProblem`1> and declare your variables:

```csharp
var problem = new SatProblem<string>();

var a = problem.AddVariable("a");
var b = problem.AddVariable("b");
var c = problem.AddVariable("c");
var d = problem.AddVariable("d");
```

Add constraints as clauses (disjunctions of literals where at least one must be true). The `!` operator negates a [Literal](xref:NanoByte.SatSolver.Literal`1):

```csharp
problem.AddClause(a, b);   // a OR b
problem.AddClause(!a, c);  // NOT a OR c
problem.AddClause(!c, d);  // NOT c OR d
problem.AddClause(a);      // a must be true (unit clause / unconditional fact)
```

You can also use `|` and `&` operator expressions with `+=` to add multiple clauses at once in a compact form:

```csharp
problem += (a | b) & (!a | c) & (!c | d) & a;
```

For common implication patterns there is a dedicated helper:

```csharp
problem.Implies(a, c);  // a implies c  (equivalent to: NOT a OR c)
problem.Implies(c, d);  // c implies d
```

You can also add an at-most-one constraint to express that no two literals in a group may be true simultaneously:

```csharp
problem.AtMostOne(a, b, c);
```

Call `Solve()` to find a satisfying assignment. It returns `null` when the problem is unsatisfiable:

```csharp
Model<string>? model = problem.Solve();
if (model != null)
{
    // Satisfiable: inspect which variables were assigned true
    foreach (var value in model.SelectedValues)
        Console.WriteLine($"{value} = true");

    // Or query a specific variable
    bool? aValue = model["a"];
}
```

When the solver needs to choose which unassigned variable to branch on next, it picks one arbitrarily. You can supply a `decider` callback to <xref:NanoByte.SatSolver.SatProblem`1.Solve*> with domain-specific branching logic for better performance:

```csharp
Model<string>? model = problem.Solve(decider: () =>
{
    // Return a preferred literal to branch on, or null to let the solver decide
    return somePreferredLiteral;
})
```
