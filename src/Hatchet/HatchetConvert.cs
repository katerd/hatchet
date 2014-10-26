using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Hatchet.Extensions;

namespace Hatchet
{
    public static class HatchetConvert
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;

        public static T Deserialize<T>(ref string input)
        {
            var parser = new Parser();
            var result = parser.Parse(ref input);
            var type = typeof (T);
            return (T)DeserializeObject(result, type);
        }

        public static string Serialize(object input)
        {
            var stringBuilder = new StringBuilder();
            Serialize(input, stringBuilder, 0);
            return stringBuilder.ToString();
        }

        private static void Serialize(object input, StringBuilder stringBuilder, int indentLevel)
        {
            var inputAsString = input as string;
            if (inputAsString != null)
            {
                if (string.Equals(inputAsString, ""))
                {
                    stringBuilder.Append("\"\"");
                    return;
                }

                var containsSpaces = inputAsString.Contains(" ");
                var containsDoubleQuotes = inputAsString.Contains("\"");
                var containsSingleQuotes = inputAsString.Contains("'");
                var containsNewLines = inputAsString.Contains("\r") || inputAsString.Contains("\n");

                if (containsNewLines)
                {
                    stringBuilder.AppendFormat("![{0}]!", inputAsString);
                    return;
                }
                if (containsDoubleQuotes && !containsSingleQuotes)
                {
                    stringBuilder.AppendFormat("'{0}'", inputAsString);
                    return;
                }
                if (containsSingleQuotes && !containsDoubleQuotes)
                {
                    stringBuilder.AppendFormat("\"{0}\"", inputAsString);
                    return;
                }
                if (containsSpaces)
                {
                    stringBuilder.AppendFormat("\"{0}\"", inputAsString.Replace("\"", "\\\""));
                    return;
                }

                stringBuilder.Append(inputAsString);
                return;

            }
            if (input is DateTime)
            {
                var inputAsDateTime = (DateTime)input;
                stringBuilder.AppendFormat("\"{0}\"", inputAsDateTime.ToString("O"));
                return;
            }

            var inputType = input.GetType();

            if (inputType.IsArray)
            {
                var inputArray = (Array) input;
                stringBuilder.AppendFormat("[{0}]", string.Join(" ", inputArray.Select(Serialize)));
                return;
            }

            var inputDictionary = input as IDictionary;
            if (inputDictionary != null)
            {
                if (inputDictionary.Count == 0)
                {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append("{");
                stringBuilder.Append(LineEnding);

                foreach (var key in inputDictionary.Keys)
                {
                    var keyStr = key.ToString();
                    if (keyStr.Contains(" "))
                    {
                        throw new HatchetException(string.Format("`{0}` is an invalid dictionary key. Key cannot contain spaces.", keyStr));
                    }

                    var value = inputDictionary[keyStr];

                    stringBuilder.Append(' ', indentLevel * IndentCount);
                    stringBuilder.Append(' ', IndentCount);
                    stringBuilder.Append(keyStr);
                    stringBuilder.Append(' ');

                    Serialize(value, stringBuilder, indentLevel + 1);
                    stringBuilder.Append(LineEnding);
                }

                stringBuilder.Append(' ', indentLevel * IndentCount);
                stringBuilder.Append("}");

                return;
            }

            if (typeof (ICollection).IsAssignableFrom(inputType))
            {
                var inputList = (ICollection) input;
                stringBuilder.AppendFormat("[{0}]", string.Join(" ", inputList.Select(Serialize)));
                return;
            }

            if (inputType.IsClass)
            {
                stringBuilder.Append("{");
                var startLength = stringBuilder.Length;
                stringBuilder.Append(LineEnding);

                var insertedValue = false;

                foreach (var field in inputType.GetFields())
                {
                    var keyStr = field.Name;
                    var value = field.GetValue(input);

                    if (value == null)
                        continue;

                    insertedValue = true;

                    stringBuilder.Append(' ', indentLevel * IndentCount);
                    stringBuilder.Append(' ', IndentCount);
                    stringBuilder.Append(keyStr);
                    stringBuilder.Append(' ');
                    Serialize(value, stringBuilder, indentLevel + 1);
                    stringBuilder.Append(LineEnding);
                }

                foreach (var property in inputType.GetProperties())
                {
                    var keyStr = property.Name;
                    var value = property.GetValue(input);

                    if (value == null)
                        continue;

                    insertedValue = true;

                    stringBuilder.Append(' ', indentLevel * IndentCount);
                    stringBuilder.Append(' ', IndentCount);
                    stringBuilder.Append(keyStr);
                    stringBuilder.Append(' ');
                    Serialize(value, stringBuilder, indentLevel + 1);
                    stringBuilder.Append(LineEnding);
                }

                if (!insertedValue)
                {
                    stringBuilder.Remove(startLength, stringBuilder.Length - startLength);
                    stringBuilder.Append("}");
                    return;
                }

                stringBuilder.Append(' ', indentLevel * IndentCount);
                stringBuilder.Append("}");

                return;
            }

            stringBuilder.Append(input);
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