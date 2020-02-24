package typing

import declarations.NamedDeclaration

enum class ValtInternalType
{
    Primitive,
    CType,
    Array,
    Struct,
    Enum,
    Map
}

enum class ResolvedTypeKind {
    Primitive, Struct, Enum, Union, Map
}
class ResolvedType
{
    var Name = ""
    var Kind = ResolvedTypeKind.Primitive
    var ElementType : ResolvedType ? = null
    var ValueType: ResolvedType? = null
    var IsPointer = false
    var IsReference = false
    var DataRef=  NamedDeclaration()

    var FixedArrayLength = -1

    var InternalType = ValtInternalType.Enum
}