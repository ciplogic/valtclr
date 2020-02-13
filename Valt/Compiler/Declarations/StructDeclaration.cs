using System;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class StructDeclaration : NamedDeclaration
    {
        private bool IsC { get; set; }
        public List<StructField> Fields { get; } = new List<StructField>();
        
        public override string ToString()
        {
            var isCStr = IsC ? "*" : "";
            return $"struct {Name}{isCStr}({String.Join(";", Fields)})";
        }


        public static StructDeclaration DeclarationEvaluation(PreModuleDeclaration structDecl)
        {
            var tokenRows = structDecl.Tokens.SplitTokensByTokenType(TokenType.Eoln);
            var strDefName = tokenRows[0][1].Text;
            var strDef = new StructDeclaration();
            if (strDefName == "C")
            {
                strDef.Name = tokenRows[0][3].Text;
                strDef.IsC = true;
            }
            else
            {
                strDef.Name = strDefName;
            }

            var modifiers = DeclarationModifiers.None;
            for (var i = 1; i < tokenRows.Length - 1; i++)
            {
                var currRow = tokenRows[i];
                if (currRow.Length == 0)
                    continue;

                if (currRow[^1].Text == ":")
                {
                    modifiers = DeclarationModifierUtilities.Parse(currRow);
                    continue;
                }
                
                var field = new StructField
                {
                    Name = currRow[0].Text, 
                    TypeTokens = currRow.Skip(1).ToArray(),
                    Modifiers = modifiers
                };
                strDef.Fields.Add(field);
            }
            return strDef;
        }
    }
}