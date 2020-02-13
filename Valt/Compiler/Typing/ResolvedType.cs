using Valt.Compiler.Declarations;

namespace Valt.Compiler.Typing
{
    public class ResolvedType
    {
        public string Name;
        public ResolvedTypeKind Kind { get; set; }
        public ResolvedType ElementType { get; set; }
        public bool IsPointer { get; set; }
        public bool IsReference { get; set; }
        public NamedDeclaration DataRef { get; set; }
        public bool IsArray { get; set; }
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
            if (IsArray)
            {
                formatData = "&";
            }
            return $"{Kind} {formatData}{Name}";
        }

    }
}