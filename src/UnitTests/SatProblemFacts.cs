// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver;

public class SatProblemFacts
{
    [Fact]
    public void SolvesSimpleSatisfiableProblem()
    {
        Literal<string> a = "a", b = "b", c = "c", d = "d";

        var problem = new SatProblem<string>();
        problem.AddClause(a, b);
        problem.AddClause(!a, c);
        problem.AddClause(!c, d);
        problem.AddClause(a);

        var model = problem.Solve();

        model.Should().NotBeNull();
        model!["a"].Should().BeTrue();
        model["c"].Should().BeTrue();
        model["d"].Should().BeTrue();
    }

    [Fact]
    public void DetectsUnsatisfiableProblem()
    {
        var problem = new SatProblem<string>();
        problem.AddClause(Literal.Of("a"));
        problem.AddClause(Literal.Of("a").Negate());

        problem.Solve().Should().BeNull();
    }

    [Fact]
    public void EmptyClauseIsUnsat()
    {
        var problem = new SatProblem<string>();
        problem.AddClause();
        problem.Solve().Should().BeNull();
    }

    [Fact]
    public void TautologicalClauseIsIgnored()
    {
        Literal<string> a = "a";
        var problem = new SatProblem<string>();
        problem.AddClause(a, !a);

        // Tautology: should not constrain anything
        var model = problem.Solve();
        model.Should().NotBeNull();
    }

    [Fact]
    public void ImpliesAddsImplicationClause()
    {
        Literal<string> a = "a", b = "b", c = "c";

        var problem = new SatProblem<string>();
        problem.AddClause(a);                  // force a=true
        problem.Implies(a, b, c);              // a → b ∨ c
        problem.AddClause(!b);                 // forbid b

        var model = problem.Solve();
        model.Should().NotBeNull();
        model!["a"].Should().BeTrue();
        model["b"].Should().BeFalse();
        model["c"].Should().BeTrue();
    }

    [Fact]
    public void DeciderBiasesSelection()
    {
        Literal<string> a = "a", b = "b";
        var problem = new SatProblem<string>();
        problem.AddClause(a, b); // at least one of a, b

        // Decider that branches by setting b=false first. Engine's fallback (no decider) would have picked a first (declaration order) and left b unconstrained.
        var model = problem.Solve(decider: () => problem.GetValue(b) is null ? !b : (Literal<string>?)null);

        model.Should().NotBeNull();
        model!["b"].Should().BeFalse();
        model["a"].Should().BeTrue();
    }

    [Fact]
    public void OperatorSyntaxSolvesSimpleSatisfiableProblem()
    {
        Literal<string> a = "a", b = "b", c = "c", d = "d";

        var problem = new SatProblem<string>();
        problem += (a | b) & (!a | c) & (!c | d) & a;

        var model = problem.Solve();

        model.Should().NotBeNull();
        model!["a"].Should().BeTrue();
        model["c"].Should().BeTrue();
        model["d"].Should().BeTrue();
    }

    [Fact]
    public void OperatorSyntaxDetectsUnsatisfiableProblem()
    {
        Literal<string> a = "a";

        var problem = new SatProblem<string>();
        problem += (Clause<string>)a;
        problem += (Clause<string>)!a;

        problem.Solve().Should().BeNull();
    }

    [Fact]
    public void SolveTwiceThrows()
    {
        var problem = new SatProblem<string>();
        problem.AddClause(Literal.Of("a"));
        problem.Solve();

        FluentActions.Invoking(() => problem.Solve())
                     .Should().Throw<System.InvalidOperationException>();
    }

    [Fact]
    public void HandlesPropagationAcrossLongChain()
    {
        // a → b → c → d → e; a forced true ⇒ all must be true.
        Literal<string> a = "a", b = "b", c = "c", d = "d", e = "e";
        var problem = new SatProblem<string>();
        problem.AddClause(a);
        problem.Implies(a, b);
        problem.Implies(b, c);
        problem.Implies(c, d);
        problem.Implies(d, e);

        var model = problem.Solve();
        model.Should().NotBeNull();
        foreach (var name in new[] {"a", "b", "c", "d", "e"})
            model![name].Should().BeTrue();
    }

    [Fact]
    public void LearnsFromConflict()
    {
        // A small problem where naive CDCL would re-explore an unsat subtree, but a CDCL solver learns and prunes.
        // We can't directly observe the learned clause, but we can check the result.
        Literal<string> a = "a", b = "b", c = "c", d = "d";
        var problem = new SatProblem<string>();
        problem.AddClause(a, b);
        problem.AddClause(!a, c);
        problem.AddClause(!a, !c);  // a ⇒ c ∧ !c (forces a=false)
        problem.AddClause(!b, d);

        var model = problem.Solve();
        model.Should().NotBeNull();
        model!["a"].Should().BeFalse();
        model["b"].Should().BeTrue();
        model["d"].Should().BeTrue();
    }
}
