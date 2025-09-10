using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hatchet.Extensions;

internal static class MemberInfoExtensions
{
    private static readonly Dictionary<MemberInfo, CustomAttributeData[]> Attributes = new();
        
    public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
    {
        if (!Attributes.TryGetValue(memberInfo, out var values))
        {                
            values = memberInfo.CustomAttributes.ToArray();
            Attributes[memberInfo] = values;
        }

        return values.Any(value => value.AttributeType == typeof(T));
    }
}
