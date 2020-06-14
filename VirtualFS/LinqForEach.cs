using System;
using System.Collections;
using System.Collections.Generic;

namespace VirtualFS
{
    public static class LinqForEach
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}