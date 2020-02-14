using Microsoft.VisualBasic;
using Valt.Compiler.Declarations;

namespace Valt.Compiler.Typing
{
    public enum ValtInternalType
    {
        Primitive = 0,
        CType,
        Array,
        Struct,
        Enum,
        Map
    }
    public class ResolvedType
    {
        public string Name;
        public ResolvedTypeKind Kind { get; set; }
        public ResolvedType ElementType { get; set; }
        public bool IsPointer { get; set; }
        public bool IsReference { get; set; }
        public NamedDeclaration DataRef { get; set; }

        public int FixedArrayLength { get; set; } = -1;

        public ValtInternalType InternalType { get; set; }
        public override string ToString()
        {
            if (DataRef != null)
                return DataRef.ToString();
            string formatData = "";
            if (IsPointer)
            {
                formatData = "*";
            }
            if (IsReference)
            {
                formatData = "&";
            }
            if (InternalType==ValtInternalType.Array)
            {
                formatData = "[]";
            }
            return $"{Kind} {formatData}{Name}";
        }

    }
}