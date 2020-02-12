using System;
using Valt.Compiler;
using Valt.Compiler.Lex;
using Xunit;

namespace Valt.XUnits
{
    public class LexerTests
    {
        [Fact]
        public void Test1()
        {
            var text = "fn test_add() {}";
            var result = Lexer.Tokenize(text);
            Assert.True("" == result.err);
            Assert.True(6 == result.items.Count);
        }
    }
}