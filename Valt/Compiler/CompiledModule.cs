using System.IO;
using Valt.Compiler.Declarations;

namespace Valt.Compiler
{
    internal class CompiledModule
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
            set { _fileName = GetFullFileName(value); }
        }

        public ModuleDeclaration Module { get; set; }

        public static string GetFullFileName(string value)
        {
            var fileInfo = new FileInfo(value);
            return fileInfo.FullName;
        }
    }
}