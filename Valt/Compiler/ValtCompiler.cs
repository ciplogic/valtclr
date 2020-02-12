using System;
using System.Collections.Generic;
using System.IO;
using Valt.Compiler.Declarations;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;
using Valt.Compiler.Typing;

namespace Valt.Compiler
{
    public class ValtCompiler
    {
        Dictionary<string, CompiledModule> modules = new Dictionary<string, CompiledModule>();
        public FileResolver FileResolver { get; } = new FileResolver();
        TypeResolver _typeResolver = new TypeResolver();
        public void CompileFile(string fileName)
        {
            var fullFileName = FileResolver.GetFullFileName(fileName);
            if (modules.ContainsKey(fullFileName))
                return;
            var content = File.ReadAllText(fullFileName);
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
 
            var moduleDefs = FirstPassCompiler.SetupDefinitions(declarations, _typeResolver);
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