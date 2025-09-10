using System;
using System.Collections.Generic;

namespace Hatchet;

public static class HatchetTypeRegistry
{
    private static readonly Dictionary<string, Type> TypeLookup = new(
        StringComparer.OrdinalIgnoreCase);

    public static void Clear()
    {
        TypeLookup.Clear();
    }

    public static void Add<T>() where T : class
    {
        var type = typeof(T);
        Add(type);
    }

    public static void Add(Type type)
    {
        var name = type.Name;

        if (!type.IsClass)
            throw new ArgumentException($"Type {type} must be a class type", nameof(type));

        if (type.IsAbstract)
            throw new ArgumentException($"Type {type} cannot be an abstract type", nameof(type));

        if (TypeLookup.ContainsKey(name))
            throw new HatchetException($"Type {type} is already registered with name {name}");

        TypeLookup[type.Name] = type;
    }

    /// <summary>
    /// Retrieve type by name, return null if no type is found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Type GetType(string name)
    {
        TypeLookup.TryGetValue(name, out var result);
        return result;
    }
}
