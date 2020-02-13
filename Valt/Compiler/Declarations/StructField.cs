using System;
using System.Linq;
using Valt.Compiler.Lex;
using Valt.Compiler.Typing;

namespace Valt.Compiler.Declarations
{
    public class StructField
    {
        public string Name;
        private Token[] _typeTokens;

        public Token[] TypeTokens
        {
            get => _typeTokens;
            set
            {
                var containsEquals = value.IndexOf(t => t.text == "=");
                if (containsEquals!=-1)
                {
                    _typeTokens = value.Take(containsEquals).ToArray();
                    ConstValue = value.Skip(containsEquals + 1).ToArray();
                    return;
                }
                
                _typeTokens = value;
                ConstValue = Array.Empty<Token>();
            }
        }

        public Token[] ConstValue { get; private set; }

        public DeclarationModifiers Modifiers { get; set; }
        public ResolvedType ResolvedType { get; set; }

        public override string ToString()
        {
            return Name + ": " + string.Join("", TypeTokens.Select(it=>it.text));
        }
    }
}