package lexing

import java.io.File
import java.lang.StringBuilder

fun MatchAll(text: String, start: Int, charMatcher: (ch: Char) -> Boolean): Int {
    for (i in start until text.length) {
        if (!charMatcher(text[i]))
            return i - start
    }

    return text.length - start
}

fun MatchAllStartLambda(
    text: String,
    start: Int,
    startMatcher: (ch: Char) -> Boolean,
    charMatcher: (ch: Char) -> Boolean
): Int {
    if (!startMatcher(text[start]))
        return 0
    return MatchAll(text, start + 1, charMatcher) + 1
}

fun matchAllStart(text: String, pos: Int, startText: String, endText: String): Int {
    if (text[pos] != startText[0]) {
        return 0
    }

    if (text.Substring(pos, startText.length) != startText) {
        return 0
    }

    val upperBound = text.length - endText.length + 1
    for (i in pos + startText.length until upperBound) {
        if (text[i] != endText[0])
            continue
        val potentialMatch = text.Substring(i, endText.length)
        if (potentialMatch == endText)
            return i + endText.length - pos
    }

    return 0
}

private fun String.Substring(start: Int, length: Int): String {
    val sb = StringBuilder(length)
    for(i in 0 until length)
        sb.append(this[start+i])
    return sb.toString()
}

fun IsAlpha(ch: Char): Boolean {
    return (ch in 'a'..'z') || (ch in 'A'..'Z') || (ch == '_') || (ch == '@')
}

fun IsNum(ch: Char): Boolean {
    return ch in '0'..'9'
}

fun IsAlphaNum(ch: Char): Boolean {
    return IsAlpha(ch) || IsNum(ch)
}

fun MatchHexNumbers(text: String, pos: Int): Int {
    if (text[pos] != '0')
        return 0
    if (pos + 2 > text.length)
        return 0
    if (text[pos + 1] != 'x')
        return 0
    for (i in pos + 2..text.length) {
        val ch = text[i]
        val isHex = IsNum(ch) || (ch in 'a'..'f') || (ch in 'A'..'F')
        if (!isHex)
            return i - pos
    }

    return text.length - pos
}

fun MatchFloatNumbers(text: String, pos: Int): Int {
    val matchInt = MatchNumbers(text, pos)
    if (matchInt == 0)
        return 0
    if (pos + matchInt == text.length)
        return matchInt
    if (text[pos + matchInt] != '.')
        return matchInt
    val matchMantissa = MatchNumbers(text, pos + matchInt + 1)
    if (matchMantissa == 0)
        return matchInt
    var expectedLen = matchInt + matchMantissa + 1
    if (pos + expectedLen == text.length)
        return expectedLen
    val potentialExponent = text[pos + expectedLen]
    if (potentialExponent != 'e' && potentialExponent != 'E')
        return expectedLen
    if (text[pos + expectedLen + 1] == '-')
        expectedLen++
    expectedLen += MatchNumbers(text, pos + expectedLen)

    return expectedLen
}

fun MatchNumbers(text: String, pos: Int): Int {
    return MatchAll(text, pos, ::IsNum)
}

fun MatchIdentifier(text: String, pos: Int): Int {
    return MatchAllStartLambda(text, pos, ::IsAlpha, ::IsAlphaNum)
}

var _reservedWords = arrayOf(
    "break", "const", "continue",
    "defer", "else", "enum", "fn",
    "for",
    "go",
    "goto",
    "if",
    "union",
    "import",
    "in",
    "interface",
    "match",
    "module",
    "mut",
    "none",
    "or",
    "pub",
    "return",
    "struct",
    "type",
    "__global"
)

fun matchReserved(text: String, pos: Int): Int {
    val matchId = MatchIdentifier(text, pos)
    if (matchId == 0)
        return 0
    for (rw in _reservedWords) {
        if (rw.length != matchId)
            continue
        if (rw[0] != text[pos])
            continue
        if (text.Substring(pos, matchId) == rw)
            return rw.length
    }

    return 0
}

fun MatchComment(text: String, pos: Int): Int {
    var len = matchAllStart(text, pos, "//", "\n")
    if (len != 0) return len - 1
    len = matchAllStart(text, pos, "/*", "*/")
    if (len == 0)
        return 0
    var openComment = 1
    for (i in pos + 2 until text.length) {
        if (text[i] == '*' && text[i + 1] == '/')
            openComment--
        if (text[i] == '/' && text[i + 1] == '*')
            openComment++
        if (openComment == 0) {
            return i + 2 - pos
        }
    }
    return 0
}

fun matchQuote(text: String, pos: Int): Int {
    val startChar = text[pos]
    val isQuote = (startChar == '\'') || (startChar == '`') || (startChar == '\"')
    if (!isQuote)
        return 0

    var isSlash = false
    for (i in pos + 1 until text.length) {
        val ch = text[i]
        if (isSlash){
            isSlash = false;
            continue
        }
        if (ch == '\\') {
            isSlash = true
            continue
        }

        if (ch == startChar) {
            val expectedLen = i - pos + 1
            return expectedLen
        }
    }

    return 0
}

fun MatchDirective(text: String, pos: Int): Int {
    if (text[pos] != '$')
        return 0
    return 1 + MatchIdentifier(text, pos + 1)
}

fun MatchPragmas(text: String, pos: Int): Int {
    return matchAllStart(text, pos, "#", "\n")
}

fun IsSpace(startChar: Char): Boolean {

    return (startChar == '\t') || (startChar == ' ')
}

fun MatchSpaces(text: String, pos: Int): Int {
    return MatchAll(text, pos, ::IsSpace)
}

var operators = arrayOf(
    "(", ")",
    "...", "..", ".",
    ";",
    ":=",
    ":", "~", "?",
    "||", "&&",
    "==",
    "=",
    "!",
    "++", "--",
    "+", "-",
    "*", "/", "%",
    "<<", ">>",
    "≠", "⩽",
    "<", ">",
    "|", "&",
    "[", "]", "{", "}",
    ",", "^"
)

fun matchOperators(text: String, pos: Int): Int {
    for (op in operators) {
        if (op[0] != text[pos])
            continue
        if (op == text.Substring(pos, op.length))
            return op.length
    }

    return 0
}

var matchers = arrayListOf<Pair<TokenType, (String, Int) -> Int>>(
    Pair(TokenType.Comment, ::MatchComment),
    Pair(TokenType.Operator, ::matchOperators),
    Pair(TokenType.Reserved, ::matchReserved),
    Pair(TokenType.Identifier, ::MatchIdentifier),
    Pair(TokenType.Eoln, ::MatchEoln),
    Pair(TokenType.Spaces, ::MatchSpaces),
    Pair(TokenType.Quote, ::matchQuote),
    Pair(TokenType.Directive, ::MatchDirective),
    Pair(TokenType.SharpPragmaOrInclude, ::MatchPragmas),
    Pair(TokenType.Number, ::MatchHexNumbers),
    Pair(TokenType.Number, ::MatchFloatNumbers),
    Pair(TokenType.Number, ::MatchNumbers)
)

fun IsEoln(ch: Char): Boolean {
    return ch == '\n' || ch == '\r'
}

fun MatchEoln(text: String, pos: Int): Int {
    return MatchAll(text, pos, ::IsEoln)
}

fun ReducedMessage(text: String, pos: Int): String {
    val remainder = text.substring(pos)
    val posEoln = remainder.IndexOf('\n')
    if (posEoln != -1) {
        return remainder.substring(0, posEoln)
    }

    return remainder
}

private fun String.IndexOf(c: Char): Int {
    for (i in 0..this.length) {
        if (this[i] == c)
            return i
    }
    return -1

}

fun IsSpaceToken(tokenType: TokenType): Boolean {
    return when (tokenType) {
        TokenType.Comment, TokenType.Spaces -> true
        else -> false
    }
}


class Lexer {
        fun Tokenize(text: String): ArrayList<Token> {
        val resultTokens = ArrayList<Token>(text.length/8)
        var pos = 0
        val tokenMatchers = matchers
        while (pos < text.length) {
            var found = false
            for (matcher in tokenMatchers) {
                val matchLen = matcher.second(text, pos)
                if (matchLen == 0)
                    continue
                found = true
                if (IsSpaceToken(matcher.first)) {
                    pos += matchLen
                    break
                }
                val foundText = text.Substring(pos, matchLen)
                pos += matchLen
                val token = Token(foundText, matcher.first)
//                println("Found: $token");
                resultTokens.add(token)
                break
            }

            if (!found) {
                val msg = ReducedMessage(text, pos)
                val countTokensToSkip = resultTokens.size - 5
                val lastTokens = resultTokens.skip(countTokensToSkip)
                println("Last tokens")
                for (lastToken in lastTokens) {
                    println("lexing.Token: $lastToken")
                }
                println("Cannot match text:'$msg")

                throw Exception("Cannot match")
            }
        }
        return resultTokens
    }
    fun tokenizeFile(fileIn: File) : ArrayList<Token> {
        val content = fileIn.readText();

        val tokens = Tokenize(content);

        return tokens;
    }

}

private fun <E> ArrayList<E>.skip(countTokensToSkip: Int): ArrayList<E> {
    val result = ArrayList<E>()
    for (i in countTokensToSkip until this.size)
        result.add(this[i])
    return result
}
