using System;
using System.Collections.Generic;
using System.Linq;

namespace Hatchet.Extensions;

internal static class ArrayExtensions
{
    public static IEnumerable<T> Select<T>(this Array array, Func<object, T> func)
    {
        return from object item in array select func(item);
    }
}
