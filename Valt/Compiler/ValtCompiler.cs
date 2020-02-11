using System.Collections.Generic;
using System.IO;
using System.Linq;
using Valt.Compiler.Declarations;
using Valt.Compiler.PrePass;

namespace Valt.Compiler
{
    public class ValtCompiler
    {
        public void CompileFile(string fileName)
        {
            var content = File.ReadAllText(fileName);
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
 
            ModuleDeclaration moduleDefs = FirstPassCompiler.SetupDefinitions(declarations);
        }
    }
}