using System.Collections.Generic;
using System.IO;
using Valt.Compiler.Declarations;

namespace Valt.Compiler
{
    public class ValtCompiler
    {
        Dictionary<string, CompiledModule> modules = new Dictionary<string, CompiledModule>();
        public FileResolver FileResolver { get; } = new FileResolver();
        public void CompileFile(string fileName)
        {
            
            var content = File.ReadAllText(fileName);
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
 
            ModuleDeclaration moduleDefs = FirstPassCompiler.SetupDefinitions(declarations);
        }
    }

    internal class CompiledModule
    {
        public string Name = "main";
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                var fileInfo = new FileInfo(value);
                _fileName = fileInfo.FullName;
            }
        }
    }
}