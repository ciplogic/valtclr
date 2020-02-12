using System;
using System.Collections.Generic;
using Valt.Compiler.Lex;

namespace Valt.Compiler.Declarations
{
    [Flags]
    public enum DeclarationModifiers
    {
        None = 0,
        Pub = 0x1,
        Mut = 0x2
    }

    public static class DeclarationModifierUtilities
    {
        public static DeclarationModifiers Parse(IEnumerable<Token> tokens)
        {
            var result = DeclarationModifiers.None;
            foreach (var token in tokens)
            {
                switch (token.text)
                {
                    case "pub":
                        result |= DeclarationModifiers.Pub;
                        break;
                    case "mut":
                        result |= DeclarationModifiers.Mut;
                        break;
                    default:
                        break;
                    
                }
            }

            return result;
        }
    }
}