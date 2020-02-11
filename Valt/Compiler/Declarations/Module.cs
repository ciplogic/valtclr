using System.Collections.Generic;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class Module : NamedDeclaration
    {
        public List<ModuleDeclaration> Items { get; } = new List<ModuleDeclaration>();
    }

    public class NamedDeclaration: ModuleDeclaration
    {
        public string Name;
    }

    public enum DeclTypes
    {
        
    }

    public class StructDeclaration : NamedDeclaration
    {
        public List<StructField> Fields { get; } = new List<StructField>();
    }

    public class StructField
    {
        public string Name;
        public Token[] TypeTokens;
    }

    public class ModuleDeclaration
    {
        
    }
}