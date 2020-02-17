using System;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Declarations;
using Valt.Compiler.Lex;

namespace Valt.Compiler.PrePass
{
    public static class FirstPassCompiler
    {
        public static Module SetupDefinitions(List<PreModuleDeclaration> declarations)
        {
            var result = new Module();
            declarations = ExtractImportDeclarations(declarations, result);
            declarations = ExtractStructDeclarations(declarations, result);
            return result;
        }

        private static List<PreModuleDeclaration> ExtractImportDeclarations(List<PreModuleDeclaration> declarations,
            Module result)
        {
            var splitOnImports = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Import);
            foreach (var importDeclaration in splitOnImports.matching)
            {
                var tokenRows = importDeclaration.Tokens
                    .SplitTokensByTokenType(TokenType.Eoln)
                    .Where(row=>row.Length>0)
                    .ToArray();
                if (tokenRows.Length == 1)
                {
                    //when importing just one item
                    var item = new ImportDeclaration
                    {
                        Name = tokenRows[0][1].Text
                    };
                    result.Imports.Add(item);
                    continue;
                }

                for (var i = 1; i < tokenRows.Length - 1; i++)
                {
                    var item = new ImportDeclaration
                    {
                        Name = tokenRows[i][0].Text
                    };
                    result.Imports.Add(item);
                }

            }

            return splitOnImports.notMatching;
        }

        private static List<PreModuleDeclaration> ExtractStructDeclarations(List<PreModuleDeclaration> declarations,
            Module result)
        {
            var splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Struct);
            declarations = splitOnStructs.notMatching;
            foreach (var structDecl in splitOnStructs.matching)
            {
                var strDef = StructDeclaration.DeclarationEvaluation(structDecl);
                result.Structs.Add(strDef);
            }
            splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Enum);
            foreach (var structDecl in splitOnStructs.matching)
            {
                var enumDeclaration = EnumDeclaration.DeclarationEvaluation(structDecl);
                result.Enums.Add(enumDeclaration);
            }
            declarations = splitOnStructs.notMatching;
            splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Module);
            foreach (var structDecl in splitOnStructs.matching)
            {
                result.Name = structDecl.Tokens[1].Text;
            }

            declarations = splitOnStructs.notMatching;
            splitOnStructs = declarations
                .FilterSplit(decl => decl.Type == ModuleDeclarationType.Function);
            foreach (var structDecl in splitOnStructs.matching)
            {
                FunctionDeclaration enumDeclaration = FunctionDeclaration.DeclarationEvaluation(structDecl);
                result.Functions.Add(enumDeclaration);
            }
            declarations = splitOnStructs.notMatching;
            return splitOnStructs.notMatching;
        }


        static int MatchRange(List<Token> tokens, int pos, string tokenText, Func<Token, bool> matchToken)
        {
            if (tokens[pos].Text != tokenText)
                return 0;
            for (var i = pos + 1; i < tokens.Count; i++)
            {
                if (matchToken(tokens[i]))
                    return i - pos + 1;
            }

            return 0;
        }

        static int MatchRange(List<Token> tokens, int pos, string tokenText, TokenType tokenType)
        {
            return MatchRange(tokens, pos, tokenText, tok => tok.Type == tokenType);
        }

        public static int MatchParenPos(IList<Token> tokens, int start, string openParen, string closeParen)
        {
            if (tokens[start].Text != openParen)
                return -1;
            var openParens = 1;
            for (var i = start + 1; i < tokens.Count; i++)
            {
                if (tokens[i].Text == openParen)
                    openParens++;
                if (tokens[i].Text == closeParen)
                    openParens--;
                if (openParens == 0)
                    return i;
            }

            return -1;
        }

        static void SetMatchRule(PreModuleDeclaration moduleDeclaration, ModuleDeclarationType declarationType,
            List<Token> tokens, int pos, int len, Token[] modifiers)
        {
            moduleDeclaration.Type = declarationType;
            var moduleTokens = new List<Token>();
            for (var i = 0; i < len; i++)
            {
                moduleTokens.Add(tokens[pos + i]);
            }

            moduleDeclaration.Tokens = moduleTokens.ToArray();
            moduleDeclaration.Modifiers = modifiers;
        }

        static bool IsSpaceToken(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Comment:
                case TokenType.Spaces:
                    return true;
                default:
                    return false;
            }
        }

        static int MatchSpaces(List<Token> tokens, int pos)
        {
            for (var i = pos; i < tokens.Count; i++)
            {
                if (!IsSpaceToken(tokens[i]))
                    return i - pos;
            }

            return tokens.Count - pos;
        }


        static int MatchModule(List<Token> tokens, int pos)
        {
            var matchLen = MatchRange(tokens, pos, "module", TokenType.Eoln);
            return matchLen;
        }

        static int MatchType(List<Token> tokens, int pos)
        {
            var matchLen = MatchRange(tokens, pos, "type", TokenType.Eoln);
            if (matchLen == 0)
                return 0;
            var lastToken = matchLen + pos - 2;
            while (tokens[lastToken].Text == "|")
            {
                var lastTokenLocal = MatchTokenWithType(tokens, lastToken + 2, TokenType.Eoln);
                lastToken = lastTokenLocal - 1;
            }
            return lastToken - pos + 1;
        }
        static int MatchEnum(List<Token> tokens, int pos)
        {
            return MatchDeclarationWithBlock(tokens, pos, "enum", "{", "}");
        }
        static int MatchGlobal(List<Token> tokens, int pos)
        {
            var matchLen = MatchRange(tokens, pos, "__global", TokenType.Eoln);
            return matchLen;
        }


        static int MatchPragma(List<Token> tokens, int pos)
        {
            switch (tokens[pos].Type)
            {
                case TokenType.SharpPragmaOrInclude:
                    return 1;
                default:
                    return 0;
            }
        }

        static int MatchDefinitionWithOptionalBlock(List<Token> tokens, int pos,
            string keyword, string openParen, string closeParen)
        {
            var matchLen = MatchRange(tokens, pos, keyword, TokenType.Eoln);
            if (matchLen == 0)
                return 0;
            var text = tokens[pos + matchLen - 2].Text;
            if (text == openParen)
            {
                var matchParenLen = MatchParenPos(tokens, pos + matchLen - 2, openParen, closeParen);
                return matchParenLen - pos + 1;
            }
            return matchLen;
        }


        static int MatchImport(List<Token> tokens, int pos)
        {
            return MatchDefinitionWithOptionalBlock(tokens, pos, "import", "(", ")");
        }

        static int MatchConst(List<Token> tokens, int pos)
        {
            return MatchDefinitionWithOptionalBlock(tokens, pos, "const", "(", ")");
        }

        static int MatchFn(List<Token> tokens, int pos)
        {
            return MatchDefinitionWithOptionalBlock(tokens, pos, "fn", "{", "}");
        }
        static int MatchInterface(List<Token> tokens, int pos)
        {
            return MatchDefinitionWithOptionalBlock(tokens, pos, "interface", "{", "}");
        }

        static int MatchTokenWithType(List<Token> tokens, int pos, TokenType tokenType)
        {
            for(var i =pos; i<tokens.Count; i++)
                if (tokens[i].Type == tokenType)
                    return i;
            return -1;
        }
        static int MatchTokenWithText(List<Token> tokens, int pos, string text)
        {
            for(var i =pos; i<tokens.Count; i++)
                if (tokens[i].Text == text)
                    return i;
            return -1;
        }

        static int MatchDeclarationWithBlock(List<Token> tokens, int pos, string startText, string openParen, string closeParen)
        {
            
            if (tokens[pos].Text != startText)
                return 0;
            var openCurly = MatchTokenWithText(tokens, pos + 1, openParen);
            var closeParenPos = MatchParenPos(tokens, openCurly, openParen, closeParen);
            return closeParenPos - pos + 1;
        }

        static int MatchStruct(List<Token> tokens, int pos)
        {
            return MatchDeclarationWithBlock(tokens, pos, "struct", "{", "}");
        }
        static int MatchUnion(List<Token> tokens, int pos)
        {
            return MatchDeclarationWithBlock(tokens, pos, "union", "{", "}");
        }

        
        

        private static (ModuleDeclarationType, Func<List<Token>, int, int>)[] _parseMatchersVec =
        {
            (ModuleDeclarationType.Spaces, MatchSpaces),
            (ModuleDeclarationType.Module, MatchModule),
            (ModuleDeclarationType.Type, MatchType),
            (ModuleDeclarationType.Global, MatchGlobal),
            (ModuleDeclarationType.Enum, MatchEnum),
            (ModuleDeclarationType.Const, MatchConst),
            (ModuleDeclarationType.Function, MatchFn),
            (ModuleDeclarationType.Struct, MatchStruct),
            (ModuleDeclarationType.Union, MatchUnion),
            (ModuleDeclarationType.Interface, MatchInterface),
            (ModuleDeclarationType.Import, MatchImport),
            (ModuleDeclarationType.Pragma, MatchPragma),
        };


        static int SkipTillNextDeclaration(List<Token> tokens, int pos)
        {
            for (var i = pos; i < tokens.Count; i++)
            {
                if (tokens[i].Type != TokenType.Eoln)
                {
                    return i;
                }
            }

            return tokens.Count;
        }

        public static List<PreModuleDeclaration> GetTopLevelDeclarations(List<Token> tokens)
        {
            var result = new List<PreModuleDeclaration>();

            var pos = 0;
            while (pos < tokens.Count)
            {
                var found = false;
                var (modifiers, newPos) = GetModifiersAtPos(tokens, pos);
                pos = newPos;
                if (pos == tokens.Count)
                {
                    break;
                }
                foreach (var rule in _parseMatchersVec)
                {
                    var matchLen = rule.Item2(tokens, pos);
                    if (matchLen == 0)
                        continue;
                    found = true;

                    var decl = new PreModuleDeclaration();
                    SetMatchRule(decl, rule.Item1, tokens, pos, matchLen, modifiers);
                    result.Add(decl);
                    pos += matchLen;
                    break;
                }

                if (!found)
                {
                    var message = "Cannot find rule at: " + tokens[pos].Text;
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
            if (tokens[startPos].Text == "[")
            {
                var endPos = MatchParenPos(tokens, startPos, "[", "]");
                for (var i = startPos; i <= endPos; i++)
                {
                    modifiers.Add(tokens[i]);
                }

                startPos = endPos+1;
            }
            while (true)
            {
                var found = false;
                switch (tokens[startPos].Text)
                {
                    case "pub":
                    case "mut":
                        found = true;
                        break;
                }

                switch (tokens[startPos].Type)
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