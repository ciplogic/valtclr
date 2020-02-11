using System;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Declarations;
using Valt.Compiler.PrePass;

namespace Valt.Compiler
{
    public static class FirstPassCompiler
    {
        public static Module SetupDefinitions(List<PreModuleDeclaration> declarations)
        {
            var result = new Module();
         
            var splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Struct);
            fillStructDeclarations(result, splitOnStructs.matching);
            declarations = splitOnStructs.notMatching.ToList();
            foreach (var moduleDeclaration in declarations)
            {
                
            }

            return result;
        }

        private static void fillStructDeclarations(Module result, List<PreModuleDeclaration> matching)
        {
            foreach (var structDecl in matching)
            {
                var strDef = StructDeclarationEvaluation(structDecl);
                result.Items.Add(strDef);
            }
        }

        private static StructDeclaration StructDeclarationEvaluation(PreModuleDeclaration structDecl)
        {
            var tokenRows = structDecl.tokens.SplitTokensByTokenType(TokenType.Eoln);
            var strDef = new StructDeclaration
            {
                Name = tokenRows[0][1].text
            };
            for (var i = 1; i < tokenRows.Count - 1; i++)
            {
                var currRow = tokenRows[i];
                if (currRow.Length == 0)
                    continue;
                
                StructField field = new StructField
                {
                    Name = currRow[0].text, 
                    TypeTokens = currRow.Skip(1).ToArray()
                };
                strDef.Fields.Add(field);
            }
            return strDef;
        }

        static int matchRange(List<Token> tokens, int pos, string tokenText, Func<Token, bool> matchToken)
        {
            if (tokens[pos].text != tokenText)
                return 0;
            for (int i = pos + 1; i < tokens.Count; i++)
            {
                if (matchToken(tokens[i]))
                    return i - pos + 1;
            }

            return 0;
        }

        static int matchRange(List<Token> tokens, int pos, string tokenText, TokenType tokenType)
        {
            return matchRange(tokens, pos, tokenText, tok => tok.type == tokenType);
        }

        static int matchParenPos(List<Token> tokens, int start, string openParen, string closeParen)
        {
            if (tokens[start].text != openParen)
                return -1;
            int openParens = 1;
            for (int i = start + 1; i < tokens.Count; i++)
            {
                if (tokens[i].text == openParen)
                    openParens++;
                if (tokens[i].text == closeParen)
                    openParens--;
                if (openParens == 0)
                    return i;
            }

            return -1;
        }
        static int matchParen(List<Token> tokens, int start, string openParen, string closeParen)
        {
            if (tokens[start].text != openParen)
                return 0;
            int openParens = 1;
            for (int i = start + 1; i < tokens.Count; i++)
            {
                if (tokens[i].text == openParen)
                    openParens++;
                if (tokens[i].text == closeParen)
                    openParens--;
                if (openParens == 0)
                    return i - start + 1;
            }

            return 0;
        }

        static int matchRange(List<Token> tokens, int pos, string tokenText, string endToken)
        {
            Func<Token, bool> matchToken = (Token tok) => tok.text == endToken;
            return matchRange(tokens, pos, tokenText, matchToken);
        }

        static void setMatchRule(PreModuleDeclaration moduleDeclaration, ModuleDeclarationType declarationType,
            List<Token> tokens, int pos, int len, Token[] modifiers)
        {
            moduleDeclaration.Type = declarationType;
            var moduleTokens = new List<Token>();
            for (int i = 0; i < len; i++)
            {
                moduleTokens.Add(tokens[pos + i]);
            }

            moduleDeclaration.tokens = moduleTokens.ToArray();
            moduleDeclaration.modifiers = modifiers;
        }

        static bool isSpaceToken(Token token)
        {
            switch (token.type)
            {
                case TokenType.Comment:
                case TokenType.Spaces:
                    return true;
                default:
                    return false;
            }
        }

        static int matchSpaces(List<Token> tokens, int pos)
        {
            for (int i = pos; i < tokens.Count; i++)
            {
                if (!isSpaceToken(tokens[i]))
                    return i - pos;
            }

            return tokens.Count - pos;
        }


        static int matchModule(List<Token> tokens, int pos)
        {
            int matchLen = matchRange(tokens, pos, "module", TokenType.Eoln);
            return matchLen;
        }

        static int matchType(List<Token> tokens, int pos)
        {
            int matchLen = matchRange(tokens, pos, "type", TokenType.Eoln);
            if (matchLen == 0)
                return 0;
            var lastToken = matchLen + pos - 2;
            while (tokens[lastToken].text == "|")
            {
                var lastTokenLocal = matchTokenWithType(tokens, lastToken + 2, TokenType.Eoln);
                lastToken = lastTokenLocal - 1;
            }
            return lastToken - pos + 1;
        }
        static int matchEnum(List<Token> tokens, int pos)
        {
            return matchDeclarationWithBlock(tokens, pos, "enum", "{", "}");
        }
        static int matchGlobal(List<Token> tokens, int pos)
        {
            int matchLen = matchRange(tokens, pos, "__global", TokenType.Eoln);
            return matchLen;
        }


        static int matchPragma(List<Token> tokens, int pos)
        {
            switch (tokens[pos].type)
            {
                case TokenType.SharpPragmaOrInclude:
                    return 1;
                default:
                    return 0;
            }
        }

        static int matchDefinitionWithOptionalBlock(List<Token> tokens, int pos,
            string keyword, string openParen, string closeParen)
        {
            int matchLen = matchRange(tokens, pos, keyword, TokenType.Eoln);
            if (matchLen == 0)
                return 0;
            string text = tokens[pos + matchLen - 2].text;
            if (text == openParen)
            {
                int matchParenLen = matchParenPos(tokens, pos + matchLen - 2, openParen, closeParen);
                return matchParenLen - pos + 1;
            }
            return matchLen;
        }


        static int matchImport(List<Token> tokens, int pos)
        {
            return matchDefinitionWithOptionalBlock(tokens, pos, "import", "(", ")");
        }

        static int matchConst(List<Token> tokens, int pos)
        {
            return matchDefinitionWithOptionalBlock(tokens, pos, "const", "(", ")");
        }

        static int matchFn(List<Token> tokens, int pos)
        {
            return matchDefinitionWithOptionalBlock(tokens, pos, "fn", "{", "}");
        }
        static int matchInterface(List<Token> tokens, int pos)
        {
            return matchDefinitionWithOptionalBlock(tokens, pos, "interface", "{", "}");
        }

        static int matchTokenWithType(List<Token> tokens, int pos, TokenType tokenType)
        {
            for(var i =pos; i<tokens.Count; i++)
                if (tokens[i].type == tokenType)
                    return i;
            return -1;
        }
        static int matchTokenWithText(List<Token> tokens, int pos, string text)
        {
            for(var i =pos; i<tokens.Count; i++)
                if (tokens[i].text == text)
                    return i;
            return -1;
        }

        static int matchDeclarationWithBlock(List<Token> tokens, int pos, string startText, string openParen, string closeParen)
        {
            
            if (tokens[pos].text != startText)
                return 0;
            var openCurly = matchTokenWithText(tokens, pos + 1, openParen);
            var closeParenPos = matchParenPos(tokens, openCurly, openParen, closeParen);
            return closeParenPos - pos + 1;
        }

        static int matchStruct(List<Token> tokens, int pos)
        {
            return matchDeclarationWithBlock(tokens, pos, "struct", "{", "}");
        }
        static int matchUnion(List<Token> tokens, int pos)
        {
            return matchDeclarationWithBlock(tokens, pos, "union", "{", "}");
        }

        
        

        private static (ModuleDeclarationType, Func<List<Token>, int, int>)[] ParseMatchersVec =
        {
            (ModuleDeclarationType.Spaces, matchSpaces),
            (ModuleDeclarationType.Module, matchModule),
            (ModuleDeclarationType.Type, matchType),
            (ModuleDeclarationType.Global, matchGlobal),
            (ModuleDeclarationType.Enum, matchEnum),
            (ModuleDeclarationType.Const, matchConst),
            (ModuleDeclarationType.Function, matchFn),
            (ModuleDeclarationType.Struct, matchStruct),
            (ModuleDeclarationType.Union, matchUnion),
            (ModuleDeclarationType.Interface, matchInterface),
            (ModuleDeclarationType.Import, matchImport),
            (ModuleDeclarationType.Pragma, matchPragma),
        };


        static int SkipTillNextDeclaration(List<Token> tokens, int pos)
        {
            for (var i = pos; i < tokens.Count; i++)
            {
                if (tokens[i].type != TokenType.Eoln)
                {
                    return i;
                }
            }

            return tokens.Count;
        }

        public static List<PreModuleDeclaration> getTopLevelDeclarations(List<Token> tokens)
        {
            List<PreModuleDeclaration> result = new List<PreModuleDeclaration>();

            int pos = 0;
            while (pos < tokens.Count)
            {
                bool found = false;
                var (modifiers, newPos) = GetModifiersAtPos(tokens, pos);
                pos = newPos;
                if (pos == tokens.Count)
                {
                    break;
                }
                foreach (var rule in ParseMatchersVec)
                {
                    int matchLen = rule.Item2(tokens, pos);
                    if (matchLen == 0)
                        continue;
                    found = true;

                    var decl = new PreModuleDeclaration();
                    setMatchRule(decl, rule.Item1, tokens, pos, matchLen, modifiers);
                    result.Add(decl);
                    pos += matchLen;
                    break;
                }

                if (!found)
                {
                    string message = "Cannot find rule at: " + tokens[pos].text;
                    Console.WriteLine(message);
                    throw new Exception(message);
                }

            }

            return result;
        }

        private static (Token[], int) GetModifiersAtPos(List<Token> tokens, in int pos)
        {
            var modifiers = new List<Token>();
            var startPos = SkipTillNextDeclaration(tokens, pos);
            if (startPos >= tokens.Count)
            {
                return (Array.Empty<Token>(), startPos);
            }
            if (tokens[startPos].text == "[")
            {
                var endPos = matchParenPos(tokens, startPos, "[", "]");
                for (var i = startPos; i <= endPos; i++)
                {
                    modifiers.Add(tokens[i]);
                }

                startPos = endPos+1;
            }
            while (true)
            {
                bool found = false;
                switch (tokens[startPos].text)
                {
                    case "pub":
                    case "mut":
                        found = true;
                        break;
                }

                switch (tokens[startPos].type)
                {
                    case TokenType.Eoln:
                        found = true;
                        break;
                }
                if (found)
                {
                    modifiers.Add(tokens[startPos]);
                    startPos++;
                    continue;
                }

                break;

            }

            return (modifiers.ToArray(), startPos);
        }
    }
}