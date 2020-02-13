using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Valt.Compiler.PrePass;
using Valt.Compiler.Typing;

namespace Valt.Compiler.Declarations
{
    public class Module : NamedDeclaration
    {
        public List<ModuleDeclaration> Items { get; } = new List<ModuleDeclaration>();
        public List<StructDeclaration> Structs { get; } = new List<StructDeclaration>();
        public List<ImportDeclaration> Imports { get; } = new List<ImportDeclaration>();

        public string[] ImportNames
        {
            get => Imports.Select(it => it.Name).ToArray();
        }

        public List<EnumDeclaration> Enums { get; } = new List<EnumDeclaration>();
        
        public void ResolveStructs(TypeResolver resolver)
        {
            resolver.RegisterTypes(Name, ResolvedTypeKind.Struct, Structs.ToArrayOfT<StructDeclaration, NamedDeclaration>());
            resolver.RegisterTypes(Name, ResolvedTypeKind.Enum, Enums.ToArrayOfT<EnumDeclaration, NamedDeclaration>());
            
            var currentModuleName = Name;
            foreach (var structDeclaration in Structs)
            {
                foreach (var structField in structDeclaration.Fields)
                {
                    var resolvedType = resolver.Resolve(structField.TypeTokens, Name);
                    Debug.Assert(resolvedType!=null);
                    structField.ResolvedType = resolvedType;
                }
                
            }
            
        }
    }

    public class ModuleDeclaration
    {
    }
}