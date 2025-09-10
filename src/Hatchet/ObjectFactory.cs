using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hatchet.Extensions;

namespace Hatchet;

internal static class ObjectFactory
{
    internal static object CreateComplexType(Type type, Dictionary<string, object> inputValues)
    {
        var withAttrs = FindConstructorsWithAttribute(type);

        var output = withAttrs.Count > 0
            ? CreateWithConstructorAttributes(inputValues, withAttrs)
            : CreateWithDefaultConstructor(type);
            
        return output;
    }

    private static List<ConstructorInfo> FindConstructorsWithAttribute(Type type) =>
        type
            .GetConstructors()
            .Where(x => x.HasAttribute<HatchetConstructorAttribute>())
            .ToList();

    private static readonly Dictionary<Type, MethodInfo> SingleStringConstructor = new();
        
    internal static MethodInfo? FindStaticConstructorMethodWithSingleStringParameter(Type type)
    {
        if (SingleStringConstructor.TryGetValue(type, out var method))
        {
            return method;
        }
            
        var scm = type.GetMethods()
            .Where(x => x.HasAttribute<HatchetConstructorAttribute>())
            .Where(x => x.IsStatic)
            .Where(x => type.IsAssignableFrom(x.ReturnType))
            .SingleOrDefault(x =>
            {
                var pc = x.GetParameters();
                if (pc.Length != 1)
                    return false;

                return pc[0].ParameterType == typeof(string);
            });

        SingleStringConstructor[type] = scm;
            
        return scm;
    }

    private static object CreateWithDefaultConstructor(Type type)
    {
        var ctor = FindDefaultConstructor(type);

        object output;

        if (!type.IsClass && ctor == null)
        {
            // structs have no default constructor
            output = Activator.CreateInstance(type);
        }
        else if (ctor != null)
        {
            output = ctor.Invoke(null);
        }
        else
        {
            throw new HatchetException($"Failed to create {type} - no constructor available");
        }

        return output;
    }

    private static ConstructorInfo? FindDefaultConstructor(Type type) =>
        type
            .GetConstructors()
            .SingleOrDefault(x => x.GetParameters().Length == 0);

    private static object CreateWithConstructorAttributes(
        IReadOnlyDictionary<string, object> inputValues, 
        IReadOnlyCollection<ConstructorInfo> withAttrs)
    {
        if (withAttrs.Count > 1)
            throw new HatchetException("Only one constructor can be tagged with [HatchetConstructor]");

        var ctor = withAttrs.First();
        var ctorParams = ctor.GetParameters();
        var args = CreateArgumentList(inputValues, ctorParams);
        var output = ctor.Invoke(args);
            
        return output;
    }

    private static object[] CreateArgumentList(
        IReadOnlyDictionary<string, object> inputValues,
        IEnumerable<ParameterInfo> ctorParams) =>
        ctorParams
            .Select(parameterInfo => inputValues[parameterInfo.Name])
            .ToArray();
}
