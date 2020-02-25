package firstpass

import lexing.Token
import lexing.TokenType


class MatchNotMatchPair<T> {
    val matching = arrayListOf<T>()
    val notNatching = arrayListOf<T>()
}

fun <T>FilterSplit(items: List<T>, filter: (T)->Boolean) : MatchNotMatchPair<T> {
    val result = MatchNotMatchPair<T>()
    for (item in items)
    {
        if (filter(item))
            result.matching.add(item);
        else
            result.notNatching.add(item)
    }
    return result
}

class TokensModifierAndPos{
    val modifiers = arrayListOf<Token>()
    val startPos = -1
}

fun SkipTillNextDeclaration(tokens: List<Token>, pos: Int): Int {
    for (i in pos until tokens.size) {
        if (tokens[i].type !== TokenType.Eoln) {
            return i
        }
    }
    return tokens.size
}

fun GetModifiersAtPos(tokens: List<Token>, pos: Int) : TokensModifierAndPos {
    val result = TokensModifierAndPos()


    return result
}