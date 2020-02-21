using System;
using System.Collections.Generic;
using System.Linq;

namespace Valt.Compiler.Lex
{
    public class Lexer
    {
        static int MatchAll(string text, int start, Func<char, bool> charMatcher)
        {
            for (var i = start; i < text.Length; i++)
            {
                if (!charMatcher(text[i]))
                    return i - start;
            }

            return text.Length - start;
        }

        static int MatchAllStart(string text, int start, Func<char, bool> startMatcher, Func<char, bool> charMatcher)
        {
            if (!startMatcher(text[start]))
                return 0;
            return MatchAll(text, start + 1, charMatcher) + 1;
        }

        static int MatchAllStart(string text, int pos, string startText, string endText)
        {
            if (text[pos] != startText[0])
            {
                return 0;
            }

            if (text.Substring(pos, startText.Length) != startText)
            {
                return 0;
            }

            var upperBound = text.Length - endText.Length + 1;
            for (var i = pos + startText.Length; i < upperBound; i++)
            {
                if (text[i] != endText[0])
                    continue;
                var potentialMatch = text.Substring(i, endText.Length);
                if (potentialMatch == endText)
                    return i + endText.Length - pos;
            }

            return 0;
        }

        static bool IsAlpha(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_')  || (ch == '@');
        }

        static bool IsNum(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        static bool IsAlphaNum(char ch)
        {
            return IsAlpha(ch) || IsNum(ch);
        }

        static int MatchHexNumbers(string text, int pos)
        {
            if (text[pos] != '0')
                return 0;
            if (pos + 2 > text.Length)
                return 0;
            if (text[pos + 1] != 'x')
                return 0;
            for (var i = pos + 2; i < text.Length; i++)
            {
                var ch = text[i];
                var isHex = Char.IsDigit(ch) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
                if (!isHex)
                    return i - pos;
            }

            return text.Length - pos;
        }

        static int MatchFloatNumbers(string text, int pos)
        {
            var matchInt = MatchNumbers(text, pos);
            if (matchInt == 0)
                return 0;
            if (pos + matchInt == text.Length)
                return matchInt;
            if (text[pos + matchInt] != '.')
                return matchInt;
            var matchMantissa = MatchNumbers(text,  pos + matchInt + 1);
            if (matchMantissa == 0)
                return matchInt;
            var expectedLen = matchInt + matchMantissa + 1;
            if (pos + expectedLen == text.Length)
                return expectedLen;
            var potentialExponent = text[pos + expectedLen];
            //var subText = text.Substring(pos);
            if (potentialExponent != 'e' && potentialExponent != 'E')
                return expectedLen;
            if (text[pos + expectedLen + 1] == '-')
                expectedLen++;
            expectedLen += MatchNumbers(text, pos + expectedLen);

            return expectedLen;
        }

        static int MatchNumbers(string text, int pos)
        {
            return MatchAll(text, pos, IsNum);
        }

        static int MatchIdentifier(string text, int pos)
        {
            return MatchAllStart(text, pos, IsAlpha, IsAlphaNum);
        }

        static string[] _reservedWords =
        {
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
        };

        static int MatchReserved(string text, int pos)
        {
            var matchId = MatchIdentifier(text, pos);
            if (matchId == 0)
                return 0;
            foreach (var rw in _reservedWords)
            {
                if (rw.Length != matchId)
                    continue;
                if (rw[0] != text[pos])
                    continue;
                if (text.Substring(pos, matchId) == rw)
                    return rw.Length;
            }

            return 0;
        }

        static int MatchComment(string text, int pos)
        {
            var len = MatchAllStart(text, pos, "//", "\n");
            if (len != 0) return len-1;
            len = MatchAllStart(text, pos, "/*", "*/");
            if (len == 0)
                return 0;
            var openComment = 1;
            for (var i = pos + 2; i < text.Length-1; i++)
            {
                if (text[i] == '*' && text[i + 1] == '/')
                    openComment--;
                if (text[i] == '/' && text[i + 1] == '*')
                    openComment++;
                if (openComment == 0)
                {
                    return i+2 - pos;
                }
            }
            return 0;
        }

        static int MatchQuote(string text, int pos)
        {
            switch (text[pos])
            {
                case '\'':
                case '`':
                case '"':
                    break;

                default:
                    return 0;
            }

            var startChar = text[pos];
            for (var i = pos + 1; i < text.Length; i++)
            {
                var ch = text[i];
                if (ch == '\\')
                {
                    i++;
                    continue;
                }

                if (ch == startChar)
                {
                    return i - pos + 1;
                }
            }

            return 0;
        }

        static int MatchDirective(string text, int pos)
        {
            if (text[pos] != '$')
                return 0;
            return 1 + MatchIdentifier(text, pos + 1);
        }

        static int MatchPragmas(string text, int pos)
        {
            return MatchAllStart(text, pos, "#", "\n");
        }

        static bool IsSpace(char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        static int MatchSpaces(string text, int pos)
        {
            return MatchAll(text, pos, IsSpace);
        }

        static string[] _operators =
        {
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
        };
        static int MatchOperators(string text, int pos)
        {
            foreach (var op in _operators)
            {
                if (op[0] != text[pos])
                    continue;
                if (op == text.Substring(pos, op.Length))
                    return op.Length;
            }

            return 0;
        }


        static bool IsEoln(char ch)
        {
            return ch == '\n' || ch == '\r';
        }

        static int MatchEoln(string text, int pos)
        {
            return MatchAll(text, pos, IsEoln);
        }

        static (TokenType, Func<string, int, int>)[] _matchers =
        {
            (TokenType.Comment, MatchComment),
            (TokenType.Operator, MatchOperators),
            (TokenType.Reserved, MatchReserved),
            (TokenType.Identifier, MatchIdentifier),
            (TokenType.Eoln, MatchEoln),
            (TokenType.Spaces, MatchSpaces),
            (TokenType.Quote, MatchQuote),
            (TokenType.Directive, MatchDirective),
            (TokenType.SharpPragmaOrInclude, MatchPragmas),
            (TokenType.Number, MatchHexNumbers),
            (TokenType.Number, MatchFloatNumbers),
            (TokenType.Number, MatchNumbers),
        };

        static string ReducedMessage(string text, int pos)
        {
            var remainder = text.Substring(pos);
            var posEoln = remainder.IndexOf('\n');
            if (posEoln != -1)
            {
                return remainder.Substring(0, posEoln);
            }

            return remainder;
        }

        static bool IsSpaceToken(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Comment:
                case TokenType.Spaces:
                    return true;
                default:
                    return false;
            }
        }

        public static (List<Token> items, string err) Tokenize(string text)
        {
            var resultTokens = new List<Token>(text.Length>>2);

            var tokenMatchers = _matchers;
            var pos = 0;
            while (pos < text.Length)
            {
                var found = false;
                foreach (var matcher in tokenMatchers)
                {
                    var matchLen = matcher.Item2(text, pos);
                    if (matchLen == 0)
                        continue;
                    found = true;
                    if (IsSpaceToken(matcher.Item1))
                    {
                        pos += matchLen;
                        break;
                    }
                    var foundText = text.Substring(pos, matchLen);
                    pos += matchLen;
                    var token = new Token() {Text = foundText, Type = matcher.Item1};
                    //Console.WriteLine("Found:"+token);
                    resultTokens.Add(token);
                    break;
                }

                if (!found)
                {
                    var msg = ReducedMessage(text, pos);
                    var countTokensToSkip = resultTokens.Count - 5;
                    var lastTokens = resultTokens.Skip(countTokensToSkip).ToArray();
                    Console.WriteLine("Last tokens");
                    foreach (var lastToken in lastTokens)
                    {
                        Console.WriteLine("Token: "+lastToken);
                    }
                    Console.WriteLine("Cannot match text:'" + msg);

                    throw new Exception("");
                }
            }

            return (resultTokens, "");
        }
    }
}