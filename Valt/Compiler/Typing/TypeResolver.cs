using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            var reservedPrimitives = new[]
            {
                "int", "bool", "string", "byte",
                "u32", "i64", "u64", "f32", "u16", "i16"
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

            var reservedPointers = new[]
            {
                "byteptr", "voidptr", "charptr"
            };
            foreach (var primitive in reservedPointers)
            {
                var resolvedType = new ResolvedType
                {
                    Name = primitive,
                    Kind = ResolvedTypeKind.Primitive,
                    IsPointer = true,
                    IsReference = false
                };
                SolvedTypes[primitive] = resolvedType;
            }
        }

        public ResolvedType Resolve(IList<Token> tokens, string packageName = "")
        {
            if (tokens.Count == 0)
                return null;
            var firstTokenText = tokens[0].text;
            if (tokens.Count == 1)
            {
                var resolvePrimitive = ResolvePrimitive(firstTokenText);
                if (resolvePrimitive != null)
                    return resolvePrimitive;
                resolvePrimitive = ResolveTypeInPackage(firstTokenText, packageName);
                if (resolvePrimitive != null)
                    return resolvePrimitive;
                Debug.Assert(false);
            }

            if (firstTokenText == "&")
            {
                return ResolveReference(tokens.Skip(1).ToArray(), packageName);
            }
            if (firstTokenText == "[" && tokens[1].text == "]")
            {
                return ResolveArray(tokens.Skip(2).ToArray(), packageName);
            }
            Console.WriteLine("Cannot resolve: "+ string.Join("", tokens.Select(t=>t.text)));
            throw new Exception("Unhandled");
        }

        private ResolvedType ResolveArray(Token[] toArray, string packageName)
        {
            var result = new ResolvedType()
            {
                IsArray = true,
                ElementType = Resolve(toArray, packageName)
            };
            return result;
        }

        private ResolvedType ResolveReference(Token[] toArray, string packageName)
        {
            var result = new ResolvedType()
            {
                IsReference = true,
                ElementType = Resolve(toArray, packageName)
            };
            return result;
        }

        private ResolvedType ResolvePrimitive(string text)
        {
            if (!SolvedTypes.TryGetValue(text, out var result))
            {
                return null;
            }

            return result;
        }
        private ResolvedType ResolveTypeInPackage(string text, string moduleName)
        {
            var fullType = moduleName.Length == 0 ? text : moduleName + "." + text;
            if (!SolvedTypes.TryGetValue(fullType, out var result))
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