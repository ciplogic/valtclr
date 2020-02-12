using System;
using System.Collections.Generic;
using System.IO;
using Valt.Compiler.Declarations;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;

namespace Valt.Compiler
{
    public class ValtCompiler
    {
        Dictionary<string, CompiledModule> modules = new Dictionary<string, CompiledModule>();
        public FileResolver FileResolver { get; } = new FileResolver();
        public void CompileFile(string fileName)
        {
            var fullFileName = FileResolver.GetFullFileName(fileName);
            if (modules.ContainsKey(fullFileName))
                return;
            var content = File.ReadAllText(fullFileName);
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
 
            Module moduleDefs = FirstPassCompiler.SetupDefinitions(declarations);
            var compiledModule = new CompiledModule(moduleDefs);
            modules[fullFileName] = compiledModule;

            CompileImports(moduleDefs.ImportNames);

        }

        private void CompileImports(string[] imports)
        {
            foreach (var importFile in imports)
            {
                var resolvedFiles = FileResolver.ResolveModule(importFile);
                foreach (var resolvedFile in resolvedFiles)
                {
                    CompileFile(resolvedFile);
                }
            }
        }
    }
}