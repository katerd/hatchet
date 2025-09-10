using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hatchet.Extensions;

internal static class MemberInfoExtensions
{
    private static readonly Dictionary<MemberInfo, CustomAttributeData[]> Attributes =
        new Dictionary<MemberInfo, CustomAttributeData[]>();
        
    public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
    {
        if (!Attributes.TryGetValue(memberInfo, out var values))
        {                
            values = memberInfo.CustomAttributes.ToArray();
            Attributes[memberInfo] = values;
        }

        for (var index = 0; index < values.Length; index++)
        {
            var value = values[index];
            if (value.AttributeType == typeof(T))
                return true;
        }

        return false;            
    }
}