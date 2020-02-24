package declarations

import lexing.Token
import typing.ResolvedType

open class Module {

}

open class NamedDeclaration : Module() {
    var name: String = "";
}

val None = 0
val Pub = 0x1
val Mut = 0x2

fun parseDeclaration(tokens: List<Token>): Int {
    var result = None;
    for (token in tokens) {
        var shiftBy = when (token.text) {
            "pub" -> Pub
            "mut" -> Mut
            else -> 0
        }
        result = result or shiftBy
    }

    return result;
}

class EnumDeclaration : NamedDeclaration() {
    var Values = arrayListOf<String>();
    var EnumValues = arrayListOf<EnumValue>();
}

class EnumValue {

}
class ParameterDeclaration : NamedDeclaration()
{
    var ArgType = ResolvedType()
}