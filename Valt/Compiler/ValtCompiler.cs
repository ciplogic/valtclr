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
                var strDef = new StructDeclaration();
                strDef.Name = structDecl.tokens[1].text;
                result.Items.Add(strDef);
            }
        }
    }
}