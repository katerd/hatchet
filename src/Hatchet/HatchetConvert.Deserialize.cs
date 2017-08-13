using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hatchet.Extensions;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private const string ClassNameKey = "Class";

        public static T Deserialize<T>(string input)
        {
            var parser = new Parser();
            var result = parser.Parse(ref input);
            var type = typeof(T);
            return (T)DeserializeObject(result, type);
        }

        private static object DeserializeObject(object result, Type type)
        {
            if (type == typeof(string))
            {
                return result;
            }

            if (type == typeof(object))
            {
                return result;
            }

            if (type.IsArray)
            {
                return DeserializeArray(result, type);
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return DeserializeDictionary(result, type);
            }

            if (IsGenericCollection(result, type))
            {
                return DeserializeGenericCollection(result, type);
            }

            if (type.IsEnum)
            {
                return DeserializeEnum(result, type);
            }

            if (IsSimpleValueType(type))
            {
                return Convert.ChangeType(result, type);
            }

            if (IsNullableValueType(type))
            {
                return DeserializeNullableValueType(result, type);
            }

            if (type == typeof(Guid))
            {
                return new Guid(result.ToString());
            }

            if (IsComplexType(type))
            {
                return GetComplexType(result, type);
            }

            throw new HatchetException($"Unable to convert {result} - unknown type {type}");
        }

        private static bool IsGenericCollection(object result, Type type)
        {
            return result is ICollection && type.IsGenericType;
        }

        private static bool IsNullableValueType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool IsComplexType(Type type)
        {
            return type.IsClass || type.IsValueType || type.IsInterface;
        }

        private static object DeserializeNullableValueType(object result, Type type)
        {
            if (result.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            var actualValue = Convert.ChangeType(result, type.GenericTypeArguments[0]);
            var nullableValue = Activator.CreateInstance(type, actualValue);
            return nullableValue;
        }

        private static object DeserializeGenericCollection(object result, Type type)
        {
            var elementType = type.GenericTypeArguments[0];

            var setG = typeof(ISet<>).MakeGenericType(elementType);

            var inputList = (List<object>) result;

            if (setG.IsAssignableFrom(type))
            {
                return DeserializeHashSet(elementType, inputList);
            }

            var listG = typeof(List<>).MakeGenericType(elementType);
            if (type.IsAssignableFrom(listG))
            {
                return DeserializeList(elementType, inputList);
            }

            throw new HatchetException($"Unable to deserialize generic collection");
        }

        private static object DeserializeList(Type elementType, List<object> inputList)
        {
            var genericListType = typeof(List<>).MakeGenericType(elementType);
            var outputList = (IList) Activator.CreateInstance(genericListType);

            foreach (var inputItem in inputList)
            {
                outputList.Add(DeserializeObject(inputItem, elementType));
            }

            return outputList;
        }

        private static object DeserializeHashSet(Type elementType, List<object> inputList)
        {
            var genericListType = typeof(HashSet<>).MakeGenericType(elementType);
            var outputList = Activator.CreateInstance(genericListType);

            foreach (var inputItem in inputList)
            {
                genericListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, outputList,
                    new[] {DeserializeObject(inputItem, elementType)});
            }

            return outputList;
        }

        private static bool IsSimpleValueType(Type type)
        {
            return type.IsPrimitive 
                   || type == typeof(decimal) 
                   || type == typeof(DateTime);
        }

        private static object DeserializeEnum(object result, Type type)
        {
            var rItems = result as ICollection;
            if (rItems != null)
            {
                if (rItems.Count == 0)
                    return 0;

                var items = rItems.Select(x => x.ToString());
                var enumStr = string.Join(",", items);
                return Enum.Parse(type, enumStr, true);
            }

            return Enum.Parse(type, (string) result, true);
        }

        private static object DeserializeDictionary(object result, Type type)
        {
            var inputDictionary = (IDictionary) result;
            var outputDictionary = (IDictionary) Activator.CreateInstance(type);

            var outputGta = outputDictionary.GetType().GetGenericArguments();
            var outputKeyType = outputGta[0];
            var outputValueType = outputGta[1];

            // todo: skip this process if the input and output dictionary generic types match

            // go through each input dictionary key and convert to the output key and value.
            foreach (var key in inputDictionary.Keys)
            {
                var newKeyValue = DeserializeObject(key, outputKeyType);

                var value = inputDictionary[key];
                var newValue = DeserializeObject(value, outputValueType);

                outputDictionary[newKeyValue] = newValue;
            }

            return outputDictionary;
        }

        private static object DeserializeArray(object result, Type type)
        {
            var arrayType = type.GetElementType();
            var inputList = (List<object>) result;
            var outputArray = Array.CreateInstance(arrayType, inputList.Count);

            for (var i = 0; i < inputList.Count; i++)
            {
                outputArray.SetValue(DeserializeObject(inputList[i], arrayType), i);
            }

            return outputArray;
        }

        private static object GetComplexType(object input, Type type)
        {
            if (input is string)
            {
                var ctor = FindConstructorWithSingleStringParameter(type);

                if (ctor != null)
                {
                    return ctor.Invoke(new[] { input });
                }

                var ctorMethod = FindStaticConstructorMethodWithSingleStringParameter(type);

                if (ctorMethod != null)
                {
                    return ctorMethod.Invoke(null, new[] {input});
                }

                throw new HatchetException($"Can't convert {input} to {type}"); 
            }

            var inputValues = (Dictionary<string, object>) input;

            type = FindComplexType(type, inputValues);

            var instance = ObjectFactory.CreateComplexType(type, inputValues);
            SetComplexTypeFields(type, inputValues, instance);
            SetComplexTypeProperties(type, inputValues, instance);

            return instance;
        }

        private static MethodInfo FindStaticConstructorMethodWithSingleStringParameter(Type type)
        {
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
            return scm;
        }

        private static ConstructorInfo FindConstructorWithSingleStringParameter(Type type)
        {
            var ctor = type.GetConstructors()
                .SingleOrDefault(x =>
                {
                    var pc = x.GetParameters();
                    if (pc.Length != 1)
                        return false;

                    return pc[0].ParameterType == typeof(string);
                });
            return ctor;
        }

        private static Type FindComplexType(Type type, Dictionary<string, object> inputValues)
        {
            if (inputValues.ContainsKey(ClassNameKey))
            {
                var name = inputValues[ClassNameKey].ToString();
                type = HatchetTypeRegistry.GetType(name);

                if (type == null)
                {
                    throw new HatchetException($"Can't create type - Type is not registered `{name}`");
                }
            }
            return type;
        }

        private static void SetComplexTypeProperties(Type type, Dictionary<string, object> inputValues, object output)
        {
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.HasAttribute<HatchetIgnoreAttribute>())
                    continue;

                var propName = prop.Name;

                if (!prop.CanWrite)
                    continue;

                if (!inputValues.ContainsKey(propName))
                    continue;

                var value = inputValues[propName];
                prop.SetValue(output, DeserializeObject(value, prop.PropertyType));
            }
        }

        private static void SetComplexTypeFields(Type type, Dictionary<string, object> inputValues, object output)
        {
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.HasAttribute<HatchetIgnoreAttribute>())
                    continue;

                var fieldName = field.Name;

                if (!inputValues.ContainsKey(fieldName))
                    continue;

                var value = inputValues[fieldName];
                field.SetValue(output, DeserializeObject(value, field.FieldType));
            }
        }
    }
}