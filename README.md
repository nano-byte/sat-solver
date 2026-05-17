# NanoByte SAT Solver

[![Build](https://github.com/nano-byte/sat-solver/actions/workflows/build.yml/badge.svg)](https://github.com/nano-byte/sat-solver/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/NanoByte.SatSolver.svg)](https://www.nuget.org/packages/NanoByte.SatSolver/)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://sat-solver.nano-byte.net/)  
NanoByte SAT Solver is a CDCL Boolean Satisfiability Solver for .NET.

## Usage

Add a reference to the [`NanoByte.SatSolver`](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package to your project.

You need to choose the underlying type to use for Literals in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. Create an instance of `SatProblem<T>` and declare your variables:

```csharp
var problem = new SatProblem<string>();

var a = problem.AddVariable("a");
var b = problem.AddVariable("b");
var c = problem.AddVariable("c");
var d = problem.AddVariable("d");
```

Add constraints as clauses (disjunctions of literals where at least one must be true). The `!` operator negates a literal:

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
    // Query a specific variable by its underlying value
    bool? aValue = model["a"];   // true, false, or null if never constrained

    // Iterate the variables you care about
    foreach (var lit in new[] {a, b, c, d})
    {
        if (model[lit.Value] == true)
            Console.WriteLine($"{lit.Value} = true");
    }
}
```

When the solver needs to choose which unassigned variable to branch on next, it picks one arbitrarily. You can supply a `decider` callback with domain-specific branching logic for better performance:

```csharp
Model<string>? model = problem.Solve(decider: () =>
{
    // Return a preferred literal to branch on, or null to let the solver decide
    return somePreferredLiteral;
});
```

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](https://gitversion.net/).

To build run `.\build.ps1` or `./build.sh` (.NET SDK is automatically downloaded if missing using [0install](https://0install.net/)).

## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrains' [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
