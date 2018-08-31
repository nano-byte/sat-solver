This website documents the API of the [NanoByte.SatSolver](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package.

NanoByte SAT Solver is a DPLL Boolean Satisfiability Solver for .NET.

This library is available for .NET Framework 2.0+ and .NET Standard 1.0+.

## Usage

First you need to choose the underlying type to use for Literals in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. You can then create an instance of `Solver<T>` as well as the Literals you wish to use:

```csharp
var solver = new Solver<string>();
var a = Literal.Of("a");
var b = Literal.Of("b");
var c = Literal.Of("c");
var d = Literal.Of("d");
```

The library enables you to express Boolean Formulas using operators and implicit casting for human-friendly sample and test code:
```csharp
var formula = (a | b) & (!a | c) & (!c | d) & a;
```

For constructing Formulas at run-time you use a collection-like interface instead:
```csharp
var formula = new Formula<string>
{
    new Clause<string> {a, b},
    new Clause<string> {a.Negate(), c},
    new Clause<string> {c.Negate(), d},
    new Clause<string> {a}
};
```

Finally, you can use the solver to determine whether a Formula is satisfiable:
```csharp
bool result = solver.IsSatisfiable(formula);
```

When the Solver needs to choose a Literal to assign a truth value to during backtracking, it simply picks the first unset Literal from the list. You can replace this with your own domain-specific logic for better performance by deriving from `Solver<T>` and overriding the `ChooseLiteral()` method.

## Building and contributing

See the [GitHub project](https://github.com/nano-byte/sat-solver) for more information.
