namespace Valt.Compiler
{
    public struct Token
    {
        public string text;
        public TokenType type;
        public override string ToString()
        {
            if (type == TokenType.Eoln)
            {
                return $"'' ({type})";
            }
            return $"'{text}' ({type})";
        }
    }
}