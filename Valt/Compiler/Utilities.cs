using System;
using System.Collections;
using System.Collections.Generic;

namespace Valt.Compiler
{
    public static class Utilities
    {
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
    }
}