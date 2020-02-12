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
            
            var reservedPrimitives = new []{"int"};
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

        public ResolvedType Resolve(List<Token> tokens)
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

        public void RegisterTypes(string moduleName, IEnumerable<string> types, ResolvedTypeKind kind,
            IList<NamedDeclaration> structs)
        {

            var pos = 0;
            foreach (var type in types)
            {
                var fullType = moduleName + "." + type;
                ResolvedType item = new ResolvedType()
                {
                    Name = fullType,
                    Kind = kind,
                    DataRef = structs[pos]
                };
                SolvedTypes[fullType] = item;
                pos++;
            }
        }
    }

    public enum ResolvedTypeKind
    {
        Primitive,
        Struct,
        Enum,
        Union
    }

    public class ResolvedType
    {
        public string Name;
        public ResolvedTypeKind Kind { get; set; }
        public ResolvedType ElementType { get; set; }
        public bool IsPointer { get; set; }
        public bool IsReference { get; set; }
        public NamedDeclaration DataRef { get; set; }
        public override string ToString()
        {
            if (DataRef != null)
                return DataRef.ToString();
            return Name + "(" + Kind + ")";
        }
    }
}