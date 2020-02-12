using System.Linq;
using Valt.Compiler.Lex;

namespace Valt.Compiler.Declarations
{
    public class StructField
    {
        public string Name;
        public Token[] TypeTokens;
        public DeclarationModifiers Modifiers { get; set; }

        public override string ToString()
        {
            return Name + ": " + string.Join("", TypeTokens.Select(it=>it.text));
        }
    }
}