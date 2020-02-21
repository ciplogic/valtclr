package lexing

enum class TokenType{
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
}
data class Token(val text: String, val kind: TokenType)
