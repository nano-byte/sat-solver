// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver;

public class FormulaFacts
{
    [Fact]
    public void DetectsConsistency()
    {
        Literal<string> a = "a", b = "b";

        (a & a).IsConsistent.Should().BeTrue();
        (a & b).IsConsistent.Should().BeTrue();
        (a & !b).IsConsistent.Should().BeTrue();
        (a & !a).IsConsistent.Should().BeFalse(because: "Conflicting literal");
        (a & (a | b)).IsConsistent.Should().BeFalse(because: "Non-literal clause");
    }

    [Fact]
    public void PropagatesUnits()
    {
        Literal<string> a = "a", b = "b", c = "c", d = "d";

        ((a | b) & (!a | c) & (!c | d) & a)
           .PropagateUnits()
           .Should().Equal(c & (!c | d) & a);

        (c & (!c | d) & a)
           .PropagateUnits().Should().Equal(c & d & a);
    }

    [Fact]
    public void EliminatesPureLiterals()
    {
        Literal<string> a = "a", b = "b", c = "c";

        ((a | b) & (!b | c) & (!c | a))
           .EliminatePureLiterals()
           .Should().Equal(!b | c);
    }

    [Fact]
    public void Simplifies()
    {
        Literal<string> a = "a", b = "b", c = "c", d = "d";

        ((a | b) & (!a | c) & (!c | d) & a)
           .Simplify()
           .Should().BeEmpty();
    }

    [Fact]
    public void SimplifiesCollectionStyle()
    {
        new Formula<string>
            {
                new Clause<string> {Literal.Of("a"), Literal.Of("b")},
                new Clause<string> {Literal.Of("a").Negate(), Literal.Of("c")},
                new Clause<string> {Literal.Of("c").Negate(), Literal.Of("d")},
                new Clause<string> {Literal.Of("a")}
            }
           .Simplify()
           .Should().BeEmpty();
    }
}
