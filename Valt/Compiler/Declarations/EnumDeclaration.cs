using System.Collections.Generic;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class EnumDeclaration : NamedDeclaration
    {
        public List<string> Values { get; } = new List<string>();

        public static EnumDeclaration DeclarationEvaluation(PreModuleDeclaration structDecl)
        {
            var tokenRows = structDecl.Tokens.SplitTokensByTokenType(TokenType.Eoln);
            var strDef = new EnumDeclaration
            {
                Name = tokenRows[0][1].Text
            };  
            for (var i = 1; i < tokenRows.Length - 1; i++)
            {
                var currRow = tokenRows[i];
                if (currRow.Length == 0)
                    continue;
                
                strDef.Values.Add(currRow[0].Text);
            }
            return strDef;
        }

        public override string ToString()
            => $"enum {Name}({string.Join(", ", Values)})";
    }
}