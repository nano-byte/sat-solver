# Advanced usage

## Multi-consequent implications

<xref:NanoByte.SatSolver.SatProblem`1.Implies*> accepts any number of consequents. The semantics are *antecedent → at-least-one-of(consequents)*, i.e. the clause `!antecedent ∨ c1 ∨ c2 ∨ ...`:

```csharp
problem.Implies(a, b, c, d);   // a → (b ∨ c ∨ d)
```

## Adding pre-built formulas

If you assemble a `Formula<T>` separately (see [Building formulas with operators](formulas.md)), feed every clause it contains to the problem in one call with <xref:NanoByte.SatSolver.SatProblem`1.AddFormula*>:

```csharp
Formula<string> formula = (a | b) & (!a | c) & (!c | d);
problem.AddFormula(formula);
```

The `problem += formula` operator does the same thing.

## Inspecting partial assignments inside a decider

The `decider` callback runs in the middle of the search, when many variables are still undecided. <xref:NanoByte.SatSolver.SatProblem`1.GetValue*> reports the engine's current truth value for a literal (or `null` if it has not been assigned yet), so the decider can react to what the search has already committed to:

```csharp
Model<string>? model = problem.Solve(decider: () =>
{
    if (problem.GetValue(a) == true)
        return c;   // prefer branching on c once a has been forced true
    return null;    // no preference: solver picks arbitrarily, then sets remaining vars to false
});
```

## Driving deciders from at-most-one constraints

<xref:NanoByte.SatSolver.SatProblem`1.AtMostOne*> returns the constraint object so you can keep a reference for use inside a decider. Two members are particularly useful:

- <xref:NanoByte.SatSolver.AtMostOneConstraint`1.Selected>: the literal in the group that has already been forced true, or `null` if none has.
- <xref:NanoByte.SatSolver.AtMostOneConstraint`1.BestUndecided*>: the first still-undecided literal in *declaration order*. List your candidates from most-preferred to least-preferred to bias the search toward the first one that is still viable.

```csharp
var version = problem.AtMostOne(v3, v2, v1);   // prefer the newest version

Model<string>? model = problem.Solve(decider: () =>
{
    if (version.Selected != null) return null;        // already pinned, nothing to do
    return version.BestUndecided();                    // try the highest-preference candidate still alive
});
```
