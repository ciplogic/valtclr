using System;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Declarations;
using Valt.Compiler.Lex;
using Valt.Compiler.PrePass;

namespace Valt.Compiler.Typing
{
    public class TypeResolver
    {
        private Dictionary<string, ResolvedType> SolvedTypes { get; } = new Dictionary<string, ResolvedType>();

        public TypeResolver()
        {
            RegisterPrimitiveTypes();
        }

        private void RegisterPrimitiveTypes()
        {
            PrimtiveTypeRegisterer.RegisterPrimitives(SolvedTypes);
        }

        public ResolvedType Resolve(IList<Token> tokens, string packageName = "")
        {
            if (tokens.Count == 0)
                return null;
            var firstTokenText = tokens[0].Text;
            if (tokens.Count == 1)
            {
                var resolvePrimitive = ResolvePrimitive(firstTokenText);
                if (resolvePrimitive != null)
                    return resolvePrimitive;
                resolvePrimitive = ResolveTypeInPackage(firstTokenText, packageName);
                if (resolvePrimitive != null)
                    return resolvePrimitive;
            }

            switch (firstTokenText)
            {
                case "&":
                    return ResolveReference(tokens.Skip(1).ToArray(), packageName);
                case "[" when tokens[1].Text == "]":
                    return ResolveArray(tokens.Skip(2).ToArray(), packageName);
                case "[" when tokens[1].Type == TokenType.Number:
                    return ResolveFixedArray(tokens.Skip(3).ToArray(), packageName, int.Parse(tokens[1].Text));
                case "C" when tokens[1].Text == ".":
                    return ResolveAsCType(tokens);
                case "map" when tokens[1].Text == "[":
                    return ResolveMap(tokens, packageName);
                default:
                    Console.WriteLine("Cannot resolve: "+ string.Join("", tokens.Select(t=>t.Text)));
                    throw new Exception("Unhandled");
            }
        }

        private static ResolvedType ResolveAsCType(IList<Token> tokens)
        {
            return new ResolvedType()
            {
                Name = "C." + tokens[2].Text,
                InternalType = ValtInternalType.CType
            };
        }

        private ResolvedType ResolveMap(IList<Token> tokens, string packageName)
        {
            var range = FirstPassCompiler.MatchParenPos(tokens, 1, "[", "]");
            var keyType = tokens.Skip(2).Take(range-2).ToArray();
            var valueType = tokens.Skip(1 + range).ToArray();
            var result = new ResolvedType()
            {
                Kind = ResolvedTypeKind.Map,
                ElementType = Resolve(keyType, packageName),
                ValueType = Resolve(valueType, packageName)
            };
            return result;
        }

        private ResolvedType ResolveFixedArray(IList<Token> toArray, string packageName, int sizeArray)
        {
            var result = new ResolvedType()
            {
                InternalType = ValtInternalType.Array,
                FixedArrayLength = sizeArray,
                ElementType = Resolve(toArray, packageName)
            };
            return result;
        }

        private ResolvedType ResolveArray(Token[] toArray, string packageName)
        {
            var result = new ResolvedType()
            {
                InternalType = ValtInternalType.Array,
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
            return SolvedTypes.TryGetValue(text, out var result) ? result : null;
        }
        private ResolvedType ResolveTypeInPackage(string text, string moduleName)
        {
            var fullType = moduleName.Length == 0 ? text : moduleName + "." + text;
            return SolvedTypes.TryGetValue(fullType, out var result) ? result : null;
        }

        public void RegisterTypes(string moduleName, ResolvedTypeKind kind,
            IList<NamedDeclaration> structs)
        {
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