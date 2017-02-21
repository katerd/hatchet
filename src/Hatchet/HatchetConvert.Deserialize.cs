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
                return GetComplexType(result, type);
            }
            
            throw new HatchetException($"Unable to convert {result} - unknown type {type}");
        }

        private static object GetComplexType(object result, Type type)
        {
            if (result is string)
            {
                throw new HatchetException($"Can't convert {type} to {result}");
            }

            var inputValues = (Dictionary<string, object>) result;

            type = FindComplexType(type, inputValues);

            var instance = CreateComplexType(type, inputValues);
            SetComplexTypeFields(type, inputValues, instance);
            SetComplexTypeProperties(type, inputValues, instance);

            return instance;
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

        private static object CreateComplexType(Type type, Dictionary<string, object> inputValues)
        {
            object output;

            var ctors = type.GetConstructors();

            var withAttrs = ctors.Where(x => x.HasAttribute<HatchetConstructorAttribute>())
                .ToList();

            if (withAttrs.Count > 0)
            {
                if (withAttrs.Count > 1)
                    throw new HatchetException("Only one constructor can be tagged with [HatchetConstructor]");

                var ctor = withAttrs.First();
                var ctorParams = ctor.GetParameters();

                var args = new List<object>();

                foreach (var parameterInfo in ctorParams)
                {
                    var argValue = inputValues[parameterInfo.Name];
                    args.Add(argValue);
                }

                output = ctor.Invoke(args.ToArray());
            }
            else
            {
                // find default constructor
                var singleCtor = ctors.SingleOrDefault(x => x.GetParameters().Length == 0);

                if (!type.IsClass && singleCtor == null)
                {
                    // structs have no default constructor
                    output = Activator.CreateInstance(type);
                }
                else if (singleCtor != null)
                {
                    output = singleCtor.Invoke(null);
                }
                else
                {
                    throw new HatchetException($"Failed to create {type} - no constructor available");
                }
            }
            return output;
        }
    }

}