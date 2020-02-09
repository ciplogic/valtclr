using System;
using System.Collections.Generic;
using System.Linq;

namespace Valt.Compiler
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

    public class Lexer
    {

        static int matchAll(string text, int start, Func<char, bool> charMatcher)
        {
            for (int i = start; i < text.Length; i++)
            {
                if (!charMatcher(text[i]))
                    return i - start;
            }

            return text.Length - start;
        }

        static int matchAllStart(string text, int start, Func<char, bool> startMatcher, Func<char, bool> charMatcher)
        {
            if (!startMatcher(text[start]))
                return 0;
            return matchAll(text, start + 1, charMatcher) + 1;
        }

        static int matchAllStart(string text, int pos, string startText, string endText)
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
            for (int i = pos + startText.Length; i < upperBound; i++)
            {
                if (text[i] != endText[0])
                    continue;
                string potentialMatch = text.Substring(i, endText.Length);
                if (potentialMatch == endText)
                    return i + endText.Length - pos;
            }

            return 0;
        }

        static bool isAlpha(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_')  || (ch == '@');
        }

        static bool isNum(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        static bool isAlphaNum(char ch)
        {
            return isAlpha(ch) || isNum(ch);
        }


        static int matchNumbers(string text, int pos)
        {
            return matchAll(text, pos, isNum);
        }

        static int matchIdentifier(string text, int pos)
        {
            return matchAllStart(text, pos, isAlpha, isAlphaNum);
        }

        static string[] ReservedWords =
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

        static int matchReserved(string text, int pos)
        {
            int matchId = matchIdentifier(text, pos);
            if (matchId == 0)
                return 0;
            foreach (var rw in ReservedWords)
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

        static int matchComment(string text, int pos)
        {
            var len = matchAllStart(text, pos, "//", "\n");
            if (len != 0) return len-1;
            len = matchAllStart(text, pos, "/*", "*/");
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

        static int matchQuote(string text, int pos)
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

            char startChar = text[pos];
            for (int i = pos + 1; i < text.Length; i++)
            {
                char ch = text[i];
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

        static int matchDirective(string text, int pos)
        {
            if (text[pos] != '$')
                return 0;
            return 1 + matchIdentifier(text, pos + 1);
        }

        static int matchPragmas(string text, int pos)
        {
            return matchAllStart(text, pos, "#", "\n");
        }

        static bool isSpace(char ch)
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

        static int matchSpaces(string text, int pos)
        {
            return matchAll(text, pos, isSpace);
        }

        static string[] operators =
        {
            "(", ")",
            "..", ".",
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

        static bool isEoln(char ch)
        {
            return ch == '\n' || ch == '\r';
        }

        static int matchEoln(string text, int pos)
        {
            return matchAll(text, pos, isEoln);
        }

        static int matchOperators(string text, int pos)
        {
            foreach (var op in operators)
            {
                if (op[0] != text[pos])
                    continue;
                if (op == text.Substring(pos, op.Length))
                    return op.Length;
            }

            return 0;
        }

        static (TokenType, Func<string, int, int>)[] matchers =
        {
            (TokenType.Comment, matchComment),
            (TokenType.Operator, matchOperators),
            (TokenType.Reserved, matchReserved),
            (TokenType.Identifier, matchIdentifier),
            (TokenType.Eoln, matchEoln),
            (TokenType.Spaces, matchSpaces),
            (TokenType.Quote, matchQuote),
            (TokenType.Directive, matchDirective),
            (TokenType.SharpPragmaOrInclude, matchPragmas),
            (TokenType.Number, matchNumbers),
        };

        static string reducedMessage(string text, int pos)
        {
            string remainder = text.Substring(pos);
            var posEoln = remainder.IndexOf('\n');
            if (posEoln != -1)
            {
                return remainder.Substring(0, posEoln);
            }

            return remainder;
        }

        static bool isSpaceToken(TokenType tokenType)
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
            List<Token> resultTokens = new List<Token>(text.Length>>2);

            var tokenMatchers = matchers;
            int pos = 0;
            while (pos < text.Length)
            {
                bool found = false;
                foreach (var matcher in tokenMatchers)
                {
                    int matchLen = matcher.Item2(text, pos);
                    if (matchLen == 0)
                        continue;
                    found = true;
                    if (isSpaceToken(matcher.Item1))
                    {
                        pos += matchLen;
                        break;
                    }
                    string foundText = text.Substring(pos, matchLen);
                    pos += matchLen;
                    var token = new Token() {text = foundText, type = matcher.Item1};
                    //Console.WriteLine("Found:"+token);
                    resultTokens.Add(token);
                    break;
                }

                if (!found)
                {
                    var msg = reducedMessage(text, pos);
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