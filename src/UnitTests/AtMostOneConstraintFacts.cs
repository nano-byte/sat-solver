// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver;

public class AtMostOneConstraintFacts
{
    [Fact]
    public void ForcesSiblingsFalseWhenOneSelected()
    {
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        problem.AtMostOne(a, b, c);
        problem.AddClause(a); // force a=true

        var model = problem.Solve();

        model.Should().NotBeNull();
        model!["a"].Should().BeTrue();
        model["b"].Should().BeFalse();
        model["c"].Should().BeFalse();
    }

    [Fact]
    public void DetectsViolation()
    {
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        problem.AtMostOne(a, b, c);
        problem.AddClause(a);
        problem.AddClause(b);

        problem.Solve().Should().BeNull();
    }

    [Fact]
    public void AllowsZeroSelected()
    {
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        problem.AtMostOne(a, b, c);
        problem.AddClause(!a);
        problem.AddClause(!b);
        problem.AddClause(!c);

        var model = problem.Solve();
        model.Should().NotBeNull();
        model!["a"].Should().BeFalse();
        model["b"].Should().BeFalse();
        model["c"].Should().BeFalse();
    }

    [Fact]
    public void CombinesWithExactlyOneClause()
    {
        // Idiomatic "exactly one of": AtMostOne + an at-least-one clause.
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        problem.AtMostOne(a, b, c);
        problem.AddClause(a, b, c); // at least one

        // Without further constraints any single literal would satisfy. Pin a=false to flush b xor c.
        problem.AddClause(!a);

        var model = problem.Solve();
        model.Should().NotBeNull();
        var trueCount = 0;
        foreach (var name in new[] {"a", "b", "c"})
            if (model![name] == true) trueCount++;
        trueCount.Should().Be(1);
    }

    [Fact]
    public void BestUndecidedReturnsFirstUnsetInOrder()
    {
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        var amo = problem.AtMostOne(a, b, c);

        // Before any solve, all are undecided.
        amo.BestUndecided().Should().Be(a);

        problem.AddClause(!a); // force a false
        // The decider is fired during Solve(); we capture the undecided literal each step.
        Literal<string>? captured = null;
        problem.Solve(decider: () =>
        {
            captured ??= amo.BestUndecided();
            return null; // let engine fall back to false-fill
        });

        captured.Should().Be(b, because: "After a is false, the next undecided literal in declaration order is b.");
    }

    [Fact]
    public void SelectedReflectsCurrentState()
    {
        Literal<string> a = "a", b = "b", c = "c";
        var problem = new SatProblem<string>();
        var amo = problem.AtMostOne(a, b, c);
        problem.AddClause(b); // force b true

        var model = problem.Solve();
        model.Should().NotBeNull();
        amo.Selected.Should().Be(b);
    }

    [Fact]
    public void ScalesToManyLiterals()
    {
        // The whole point of the primitive: avoid n² binary clauses.
        const int n = 100;
        var problem = new SatProblem<int>();
        var lits = new Literal<int>[n];
        for (int i = 0; i < n; i++) lits[i] = problem.AddVariable(i);
        problem.AtMostOne(lits);
        problem.AddClause(lits[42]); // force one specific literal true

        var model = problem.Solve();
        model.Should().NotBeNull();
        for (int i = 0; i < n; i++)
            model![i].Should().Be(i == 42);
    }
}
