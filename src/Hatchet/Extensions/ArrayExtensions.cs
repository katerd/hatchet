using System;
using System.Collections.Generic;

namespace Hatchet.Extensions
{
    internal static class ArrayExtensions
    {
        public static IEnumerable<T> Select<T>(this Array array, Func<object, T> func)
        {
            foreach (var item in array)
            {
                yield return func(item);
            }
        }
    }
}