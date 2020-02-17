using System.Collections.Generic;

namespace Valt.Compiler.Typing
{
    class PrimtiveTypeRegisterer
    {
        public static void RegisterPrimitives(Dictionary<string, ResolvedType> SolvedTypes)
        {
            
            var reservedPrimitives = new[]
            {
                "int", "bool", "string", "byte", "char",
                "u32", "i64", "u64", "f32", "f64", "u16", "i16"
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
    }
}