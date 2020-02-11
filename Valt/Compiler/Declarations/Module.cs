using System;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Declarations
{
    public class Module : NamedDeclaration
    {
        public List<ModuleDeclaration> Items { get; } = new List<ModuleDeclaration>();
        public List<ImportDeclaration> Imports { get;  }= new List<ImportDeclaration>();
    }

    public class ImportDeclaration: NamedDeclaration
    {
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
        public override string ToString() 
            => $"struct {Name}({String.Join(";", Fields)})";
    }

    public class StructField
    {
        public string Name;
        public Token[] TypeTokens;
        public override string ToString()
        {
            return Name + ": " + string.Join("", TypeTokens.Select(it=>it.text));
        }
    }

    public class ModuleDeclaration
    {
        
    }
}