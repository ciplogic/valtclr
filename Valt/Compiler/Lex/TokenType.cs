namespace Valt.Compiler.Lex
{
    public enum TokenType
    {
        Identifier,
        Number,
        Spaces,
        Eoln,
        Quote,
        Reserved,
        Operator,
        Comment,
        SharpPragmaOrInclude,
        Directive,
    };
}