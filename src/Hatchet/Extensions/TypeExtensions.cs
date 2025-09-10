using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hatchet.Extensions;

internal static class TypeExtensions
{
    public static IEnumerable<PropertyInfo> GetPropertiesToSerialize(this Type inputType)
    {
        return GetNonIgnoredProperties(inputType)
            .Where(x => x.SetMethod != null);
    }

    public static IEnumerable<PropertyInfo> GetNonIgnoredProperties(this Type inputType)
    {
        return inputType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => !x.HasAttribute<HatchetIgnoreAttribute>());
    }

    public static IEnumerable<FieldInfo> GetFieldsToSerialize(this Type inputType)
    {
        return inputType.GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => !x.HasAttribute<HatchetIgnoreAttribute>());
    }
}