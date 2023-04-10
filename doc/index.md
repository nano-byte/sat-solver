---
title: Home
---

# NanoByte SAT Solver

NanoByte SAT Solver is a DPLL Boolean Satisfiability Solver for .NET.

## Usage

Add a reference to the [NanoByte.SatSolver](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package to your project. It is available for .NET Framework 2.0+ and .NET Standard 1.0+.

You need to choose the underlying type to use for [Literals](xref:NanoByte.SatSolver.Literal`1) in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. You can then create an instance of <xref:NanoByte.SatSolver.Solver`1>:

```csharp
var solver = new Solver<string>();
```

The library enables you to express Boolean [Formulas](xref:NanoByte.SatSolver.Formula`1) using implicit casting and operators for human-friendly sample and test code:
```csharp
Literal<string> a = "a", b = "b", c = "c", d = "d";
var formula = (a | b) & (!a | c) & (!c | d) & a;
```

For constructing Formulas at run-time you can use a collection-like interface instead:
```csharp
var formula = new Formula<string>
{
    new Clause<string> {Literal.Of("a"), Literal.Of("b")},
    new Clause<string> {Literal.Of("a").Negate(), Literal.Of("c")},
    new Clause<string> {Literal.Of("c").Negate(), Literal.Of("d")},
    new Clause<string> {Literal.Of("a")}
};
```

Finally, you can use the solver to determine whether a Formula is satisfiable:
```csharp
bool result = solver.IsSatisfiable(formula);
```

When the Solver needs to choose a Literal to assign a truth value to during backtracking, it simply picks the first unset Literal from the list. You can replace this with your own domain-specific logic for better performance by deriving from <xref:NanoByte.SatSolver.Solver`1> and overriding the [ChooseLiteral()](xref:NanoByte.SatSolver.Solver`1#NanoByte_SatSolver_Solver_1_ChooseLiteral_NanoByte_SatSolver_Formula__0__) method.
