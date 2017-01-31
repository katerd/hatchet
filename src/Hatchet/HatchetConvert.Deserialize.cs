using System;
using System.Collections;
using System.Collections.Generic;
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
                var arrayType = type.GetElementType();
                var inputList = (List<object>)result;
                var outputArray = Array.CreateInstance(arrayType, inputList.Count);

                for (var i = 0; i < inputList.Count; i++)
                {
                    outputArray.SetValue(DeserializeObject(inputList[i], arrayType), i);
                }
                return outputArray;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var inputDictionary = (IDictionary)result;
                var outputDictionary = (IDictionary)Activator.CreateInstance(type);

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

            if (result is ICollection)
            {
                if (type.IsGenericType)
                {
                    var elementType = type.GenericTypeArguments[0];

                    var setG = typeof(ISet<>).MakeGenericType(elementType);

                    var inputList = (List<object>)result;

                    if (setG.IsAssignableFrom(type))
                    {
                        var genericListType = typeof(HashSet<>).MakeGenericType(elementType);
                        var outputList = Activator.CreateInstance(genericListType);

                        foreach (var inputItem in inputList)
                        {
                            genericListType.InvokeMember("Add", BindingFlags.InvokeMethod, null, outputList,
                                new[] { DeserializeObject(inputItem, elementType) });
                        }

                        return outputList;
                    }

                    var listG = typeof(List<>).MakeGenericType(elementType);

                    if (type.IsAssignableFrom(listG))
                    {
                        var genericListType = typeof(List<>).MakeGenericType(elementType);
                        var outputList = (IList)Activator.CreateInstance(genericListType);

                        foreach (var inputItem in inputList)
                        {
                            outputList.Add(DeserializeObject(inputItem, elementType));
                        }

                        return outputList;
                    }
                }
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, (string)result, true);
            }

            if (type.IsPrimitive 
                || type == typeof(decimal) 
                || type == typeof(DateTime))
            {
                return Convert.ChangeType(result, type);
            }

            if (type == typeof(Guid))
            {
                return new Guid(result.ToString());
            }

            if (type.IsClass || type.IsValueType)
            {
                return CreateComplexType(result, type);
            }
            
            throw new HatchetException($"Unable to convert {result} - unknown type {type}");
        }

        private static object CreateComplexType(object result, Type type)
        {
            if (result is string)
            {
                throw new HatchetException($"Can't convert {type} to {result}");
            }

            var inputValues = (Dictionary<string, object>) result;

            if (inputValues.ContainsKey(ClassNameKey))
            {
                var name = inputValues[ClassNameKey].ToString();
                type = HatchetTypeRegistry.GetType(name);

                if (type == null)
                {
                    throw new HatchetException($"Can't create type - Type is not registered `{name}`");
                }
            }

            object output;

            try
            {
                output = Activator.CreateInstance(type);
            }
            catch (MissingMethodException missingMethodException)
            {
                throw new HatchetException($"Failed to create {type} - no constructor available",
                    missingMethodException);
            }

            var fields = type.GetFields();
            var props = type.GetProperties();

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

            foreach (var prop in props)
            {
                if (prop.HasAttribute<HatchetIgnoreAttribute>())
                    continue;

                var propName = prop.Name;

                if (!inputValues.ContainsKey(propName))
                    continue;

                var value = inputValues[propName];
                prop.SetValue(output, DeserializeObject(value, prop.PropertyType));
            }

            return output;
        }
    }

}