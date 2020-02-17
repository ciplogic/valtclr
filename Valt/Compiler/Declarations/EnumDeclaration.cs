using System;
using System.Collections.Generic;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class EnumDeclaration : NamedDeclaration
    {
        public List<string> Values { get; } = new List<string>();
        internal List<EnumValue> EnumValues { get; } = new List<EnumValue>();

        public static EnumDeclaration DeclarationEvaluation(PreModuleDeclaration structDecl)
        {
            var tokenRows = structDecl.Tokens.SplitTokensByTokenType(TokenType.Eoln);
            var result = new EnumDeclaration
            {
                Name = tokenRows[0][1].Text
            };
            var startValue = 0;
            for (var i = 1; i < tokenRows.Length - 1; i++)
            {
                var currRow = tokenRows[i];
                if (currRow.Length == 0)
                    continue;
                if (currRow.Length == 3)
                {
                    startValue = Int32.Parse(currRow[2].Text);
                }

                var enumKey = currRow[0].Text;
                result.EnumValues.Add(new EnumValue(){Key = enumKey, Value = startValue});
                result.Values.Add(enumKey);
                startValue++;
            }
            return result;
        }

        public override string ToString()
            => $"enum {Name}({string.Join(", ", EnumValues)})";
    }
}