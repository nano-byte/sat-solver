# NanoByte SAT Solver

[![NuGet](https://img.shields.io/nuget/v/NanoByte.SatSolver.svg)](https://www.nuget.org/packages/NanoByte.SatSolver/)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://sat-solver.nano-byte.net/)
[![Build status](https://img.shields.io/appveyor/ci/nano-byte/sat-solver.svg)](https://ci.appveyor.com/project/nano-byte/sat-solver)  
NanoByte SAT Solver is a DPLL Boolean Satisfiability Solver for .NET.

## Usage

Add a reference to the [NanoByte.SatSolver](https://www.nuget.org/packages/NanoByte.SatSolver/) NuGet package to your project. It is available for .NET Framework 2.0+ and .NET Standard 1.0+.

You need to choose the underlying type to use for Literals in Boolean Formulas. This will often be `int` or `string` but you can also use any other type that implements the `IEquatable<T>` interface. You can then create an instance of `Solver<T>`:

```csharp
var solver = new Solver<string>();
```

The library enables you to express Boolean Formulas using implicit casting and operators for human-friendly sample and test code:
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

When the Solver needs to choose a Literal to assign a truth value to during backtracking, it simply picks the first unset Literal from the list. You can replace this with your own domain-specific logic for better performance by deriving from `Solver<T>` and overriding the `ChooseLiteral()` method.

## Building

The source code is in [`src/`](src/), a project for API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`.

You need [Visual Studio 2017](https://www.visualstudio.com/downloads/) to perform a full build of this project.  
You can build for .NET Standard on Linux using just the [.NET Core SDK 2.1+](https://www.microsoft.com/net/download). Additionally installing [Mono 5.10+](https://www.mono-project.com/download/stable/) allows you to also build for .NET Framework. The build scripts will automatically adjust accordingly.

Run `.\build.ps1` on Windows or `./build.sh` on Linux. These scripts take a version number as an input argument. The source code itself contains no version numbers. Instead the version is picked by continuous integration using [GitVersion](http://gitversion.readthedocs.io/).

## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrain's [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
