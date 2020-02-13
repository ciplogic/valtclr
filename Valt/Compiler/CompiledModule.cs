using System.IO;
using Valt.Compiler.Declarations;
using Valt.Compiler.Typing;

namespace Valt.Compiler
{
    public class CompiledModule
    {
        public string Name = "main";
        private string _fileName;

        public CompiledModule(Module moduleDeclaration)
        {
            Module = moduleDeclaration;
        }

        public string FileName
        {
            get => _fileName;
            set { _fileName = FileResolver.GetFullFileName(value); }
        }

        public Module Module { get; set; }

        public void ResolveStructs(TypeResolver typeResolver)
        {
            Module.ResolveStructs(typeResolver);
        }
    }
}