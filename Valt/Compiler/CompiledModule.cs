using System.IO;
using Valt.Compiler.Declarations;

namespace Valt.Compiler
{
    public class CompiledModule
    {
        public string Name = "main";
        private string _fileName;

        public CompiledModule(ModuleDeclaration moduleDeclaration)
        {
            Module = moduleDeclaration;
        }

        public string FileName
        {
            get => _fileName;
            set { _fileName = FileResolver.GetFullFileName(value); }
        }

        public ModuleDeclaration Module { get; set; }
    }
}