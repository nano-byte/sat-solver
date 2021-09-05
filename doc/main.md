NanoByte SAT Solver is a DPLL Boolean Satisfiability Solver for .NET.

[**GitHub repository**](https://github.com/nano-byte/sat-solver)

## Usage

Add a reference to the [NanoByte.SatSolver](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package to your project. It is available for .NET Framework 2.0+ and .NET Standard 1.0+.

You need to choose the underlying type to use for \ref NanoByte.SatSolver.Literal "Literals" in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. You can then create an instance of \ref NanoByte.SatSolver.Solver "Solver<T>":

```{.cs}
var solver = new Solver<string>();
```

The library enables you to express Boolean \ref NanoByte.SatSolver.Formula "Formulas" using implicit casting and operators for human-friendly sample and test code:
```{.cs}
Literal<string> a = "a", b = "b", c = "c", d = "d";
var formula = (a | b) & (!a | c) & (!c | d) & a;
```

For constructing Formulas at run-time you can use a collection-like interface instead:
```{.cs}
var formula = new Formula<string>
{
    new Clause<string> {Literal.Of("a"), Literal.Of("b")},
    new Clause<string> {Literal.Of("a").Negate(), Literal.Of("c")},
    new Clause<string> {Literal.Of("c").Negate(), Literal.Of("d")},
    new Clause<string> {Literal.Of("a")}
};
```

Finally, you can use the solver to determine whether a Formula is satisfiable:
```{.cs}
bool result = solver.IsSatisfiable(formula);
```

When the Solver needs to choose a Literal to assign a truth value to during backtracking, it simply picks the first unset Literal from the list. You can replace this with your own domain-specific logic for better performance by deriving from \ref NanoByte.SatSolver.Solver "Solver<T>" and overriding the \ref NanoByte.SatSolver.Solver.ChooseLiteral "ChooseLiteral()" method.
