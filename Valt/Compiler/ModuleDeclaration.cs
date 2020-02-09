using System.Linq;

namespace Valt.Compiler
{
    public class ModuleDeclaration
    {
        public ModuleDeclarationType Type;
        public Token[] modifiers;
        public Token[] tokens;
        public override string ToString()
        {
            var modifiersText = string.Join(" ",modifiers.Select(m => m.text));
            var tokensText = string.Join(" ", tokens.Select(m => m.text));
            return string.Join(" ",
                $"{modifiersText} {tokensText}- ({Type})");
        }
    };
}