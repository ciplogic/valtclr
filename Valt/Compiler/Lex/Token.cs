namespace Valt.Compiler
{
    public struct Token
    {
        public string text;
        public TokenType type;
        public override string ToString()
            => $"'{text}' ({type})";
    }
}