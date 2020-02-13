namespace Valt.Compiler.Lex
{
    public struct Token
    {
        public string Text;
        public TokenType Type;
        public override string ToString()
        {
            if (Type == TokenType.Eoln)
            {
                return $"'' ({Type})";
            }
            return $"'{Text}' ({Type})";
        }
    }
}