namespace Valt.Compiler.TypeSolve
{
    public enum TypeRelationToElem
    {
        IsAs,
        IsArrayOf,
        IsReferenceOf,
        IsPointerOf
    }
    public class TypeSolved
    {
        public string Name;
        public string Module;
        public TypeRelationToElem RelationToElem;
        public bool IsBuiltIn;
        public TypeSolved TargetType;
    }
}