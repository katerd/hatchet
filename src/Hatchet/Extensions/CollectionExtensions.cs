using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet.Extensions;

internal static class CollectionExtensions
{
    public static IEnumerable<T> Select<T>(this ICollection collection, Func<object, T> func)
    {
        foreach (var item in collection)
        {
            yield return func(item);
        }
    }
}