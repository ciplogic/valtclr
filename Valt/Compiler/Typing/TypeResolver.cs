using System;
using System.Collections.Generic;
using Valt.Compiler.Declarations;
using Valt.Compiler.Lex;

namespace Valt.Compiler.Typing
{
    public class TypeResolver
    {
        private Dictionary<string, ResolvedType> SolvedTypes { get; } = new Dictionary<string, ResolvedType>();
        CompiledModule ParentPrimitives = new CompiledModule(null){Name = "vroot"};

        public TypeResolver()
        {
            RegisterPrimitiveTypes();
        }

        private void RegisterPrimitiveTypes()
        {
            
            var reservedPrimitives = new []
            {
                "int", "bool", "string", "byteptr",
                "u32", "i64", "u64"
            };
            foreach (var primitive in reservedPrimitives)
            {
                var resolvedType = new ResolvedType
                {
                    Name = primitive,
                    Kind = ResolvedTypeKind.Primitive,
                    IsPointer = false,
                    IsReference = false
                };
                SolvedTypes[primitive] = resolvedType;
            }
        }

        public ResolvedType Resolve(List<Token> tokens, string packageName = "")
        {
            if (tokens.Count == 1)
            {
                var resolvePrimitive = ResolvePrimitive(tokens[0].text);
                if (resolvePrimitive == null)
                    throw new Exception("Cannot solve"+tokens[0]);
            }
            throw new Exception("Unhandled");
        }

        private ResolvedType ResolvePrimitive(string text)
        {
            if (!SolvedTypes.TryGetValue(text, out var result))
            {
                return null;
            }

            return result;
        }

        public void RegisterTypes(string moduleName, ResolvedTypeKind kind,
            IList<NamedDeclaration> structs)
        {
            if(structs.Count==0)
                return;
            foreach (var strDef in structs)
            {
                var fullType = moduleName.Length==0? strDef.Name : moduleName + "." + strDef.Name;
                SolvedTypes[fullType] = new ResolvedType()
                {
                    Name = fullType,
                    Kind = kind,
                    DataRef = strDef
                };
            }
        }
    }
}