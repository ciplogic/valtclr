namespace Valt.Compiler
{
    public class Token
    {
        public string text;
        public TokenType type;
        public override string ToString()
            => $"'{text}' ({type})";
    }
}