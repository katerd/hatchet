using System;
using System.Linq;
using System.Reflection;

namespace Hatchet.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>().Any();
        }
    }
}