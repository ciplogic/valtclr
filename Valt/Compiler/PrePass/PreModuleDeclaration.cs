using System.Linq;
using Valt.Compiler.Lex;

namespace Valt.Compiler.PrePass
{
    public class PreModuleDeclaration
    {
        public ModuleDeclarationType Type;
        public Token[] Modifiers;
        public Token[] Tokens;
        public override string ToString()
        {
            var modifiersText = string.Join(" ",Modifiers.Select(m => m.Text));
            var tokensText = string.Join(" ", Tokens.Select(m => m.Text));
            return string.Join(" ",
                $"{modifiersText} {tokensText}- ({Type})");
        }
    };
}