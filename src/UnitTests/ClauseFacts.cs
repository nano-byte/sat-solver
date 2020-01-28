// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using Xunit;

namespace NanoByte.SatSolver
{
    public class ClauseFacts
    {
        [Fact]
        public void AtMostOne()
        {
            Literal<string> a = "a", b = "b", c = "c";

            Clause.AtMostOne(a, b, c)
                  .Should().Equal((!a | !b) & (!a | !c) & (!b | !c));
        }
    }
}
