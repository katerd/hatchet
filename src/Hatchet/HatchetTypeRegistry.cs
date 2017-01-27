using System;
using System.Collections.Generic;

namespace Hatchet
{
    public static class HatchetTypeRegistry
    {
        private static readonly Dictionary<string, Type> TypeLookup = new Dictionary<string, Type>(
            StringComparer.OrdinalIgnoreCase);

        public static void Clear()
        {
            TypeLookup.Clear();
        }

        public static void Add<T>() where T : class
        {
            var type = typeof(T);
            var name = type.Name;

            if (TypeLookup.ContainsKey(name))
                throw new HatchetException($"Type {type} is already registered with name {name}");

            TypeLookup[name] = type;
        }

        /// <summary>
        /// Retrieve type by name, return null if no type is found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetType(string name)
        {
            Type result;
            TypeLookup.TryGetValue(name, out result);
            return result;
        }
    }
}