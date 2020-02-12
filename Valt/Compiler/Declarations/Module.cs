using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class Module : NamedDeclaration
    {
        public List<ModuleDeclaration> Items { get; } = new List<ModuleDeclaration>();
        public List<ModuleDeclaration> Types { get; } = new List<ModuleDeclaration>();
        public List<ImportDeclaration> Imports { get;  }= new List<ImportDeclaration>();

        public string[] ImportNames
        {
            get => Imports.Select(it => it.Name).ToArray();
        }
    }

    public class ModuleDeclaration
    {
    }
}