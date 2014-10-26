using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet
{
    public static class HatchetConvert
    {
        public static T Deserialize<T>(ref string input)
        {
            var parser = new Parser();
            var result = parser.Parse(ref input);
            var type = typeof (T);
            return (T)DeserializeObject(result, type);
        }

        public static string Serialize(object input)
        {
            throw new NotImplementedException();
        }

        private static object DeserializeObject(object result, Type type)
        {
            if (type == typeof(string))
            {
                return result;
            }
            if (type.IsArray)
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
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                if (!(result is ICollection))
                {
                    throw new HatchetException(string.Format("Expected a list but got a {0}", result.GetType().Name));
                }

                var inputList = (List<object>)result;

                var listType = type.GenericTypeArguments[0];

                var genericListType = typeof(List<>).MakeGenericType(listType);
                var outputList = (IList)Activator.CreateInstance(genericListType);

                foreach (var inputItem in inputList)
                {
                    outputList.Add(DeserializeObject(inputItem, listType));
                }

                return outputList;
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, (string)result, true);
            }
            if (type.IsClass)
            {
                var inputValues = (Dictionary<string, object>)result;

                var output = Activator.CreateInstance(type);

                var fields = type.GetFields();
                var props = type.GetProperties();

                foreach (var field in fields)
                {
                    var fieldName = field.Name;
                    if (!inputValues.ContainsKey(fieldName))
                        continue;

                    var value = inputValues[fieldName];
                    field.SetValue(output, DeserializeObject(value, field.FieldType));
                }
                foreach (var prop in props)
                {
                    var propName = prop.Name;

                    if (!inputValues.ContainsKey(propName))
                        continue;

                    var value = inputValues[propName];
                    prop.SetValue(output, DeserializeObject(value, prop.PropertyType));
                }

                return output;
            }

            return Convert.ChangeType(result, type);
        }

        

    }
}