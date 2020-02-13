using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Valt.Compiler.Lex;

namespace Valt.Compiler
{
    public static class Utilities
    {
        public static TOut[] ToArrayOfT<TIn, TOut>(this IList<TIn> items) 
            where TIn:TOut 
        {
            return items.Select(it => (TOut) it).ToArray();
        }
        public static (List<T>matching, List<T>notMatching) FilterSplit<T>(this IEnumerable<T> items,
            Func<T, bool> filter)
        {
            var matching = new List<T>();
            var notMatching = new List<T>();
            foreach (var item in items)
            {
                if (filter(item))
                {
                    matching.Add(item);
                }
                else
                {
                    notMatching.Add(item);
                }
            }
            return (matching, notMatching);
        }

        public static int IndexOf<T>(this IList<T> items, Func<T, bool> predicate)
        {
            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (predicate(item))
                    return index;
            }
            return -1;
        }
        public static Token[][] SplitTokensByTokenType(this IEnumerable<Token> tokens, TokenType tokenType, bool removeEmptyItems = true)
        {
            var result = new List<Token[]>();
            var currentRow = new List<Token>();
            foreach (var token in tokens)
            {
                if (token.type!=tokenType)
                    currentRow.Add(token);
                else
                {
                    result.Add(currentRow.ToArray());
                    currentRow.Clear();
                }
            }
            result.Add(currentRow.ToArray());
            return result
                .Where(it=>it.Length>0 || !removeEmptyItems)
                .ToArray();
        }
    }
}