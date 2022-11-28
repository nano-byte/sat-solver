// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver;

public class SolverFacts
{
    [Fact]
    public void DetectsSatisfiableFormulas()
    {
        Literal<string> a = "a", b = "b", c = "c", d = "d";
        var formula = (a | b) & (!a | c) & (!c | d) & a;

        new Solver<string>().IsSatisfiable(formula).Should().BeTrue();
    }

    [Fact]
    public void DetectsUnsatisfiableFormulas()
    {
        Literal<string> a = "a";
        var formula = a & !a;

        new Solver<string>().IsSatisfiable(formula).Should().BeFalse();
    }
}
