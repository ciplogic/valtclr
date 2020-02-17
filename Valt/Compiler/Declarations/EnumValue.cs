namespace Valt.Compiler.Declarations
{
    struct EnumValue
    {
        public string Key;
        public int Value;
        public override string ToString() 
            => $"{Key}: {Value}";
    }
}