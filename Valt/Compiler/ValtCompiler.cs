using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Declarations;
using Valt.Compiler.PrePass;

namespace Valt.Compiler
{
    public class ValtCompiler
    {
        public Module SetupDefinitions(List<PreModuleDeclaration> declarations)
        {
            var result = new Module();
         
            var splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Struct);
            fillStructDeclarations(result, splitOnStructs.matching);
            declarations = splitOnStructs.notMatching.ToList();
            foreach (var moduleDeclaration in declarations)
            {
                
            }

            return result;
        }

        private void fillStructDeclarations(Module result, List<PreModuleDeclaration> matching)
        {
            foreach (var structDecl in matching)
            {
                var strDef = StructDeclarationEvaluation(structDecl);
                result.Items.Add(strDef);
            }
        }

        private static StructDeclaration StructDeclarationEvaluation(PreModuleDeclaration structDecl)
        {
            var tokenRows = structDecl.tokens.SplitTokensByTokenType(TokenType.Eoln);
            var strDef = new StructDeclaration();
            strDef.Name = tokenRows[0][1].text;
            for (var i = 1; i < tokenRows.Count - 1; i++)
            {
                var currRow = tokenRows[i];
                StructField field = new StructField
                {
                    Name = currRow[0].text, 
                    TypeTokens = currRow.Skip(1).ToArray()
                };
                strDef.Fields.Add(field);
            }
            return strDef;
        }
    }
}