// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver
{
    public class ClausesFacts
    {
        [Fact]
        public void AtMostOne()
        {
            Literal<string> a = "a", b = "b", c = "c";

            Clauses.AtMostOne(a, b, c)
                  .Should().Equal((!a | !b) & (!a | !c) & (!b | !c));
        }
    }
}
