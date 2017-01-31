using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hatchet.Extensions;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;

        public static string Serialize(object input)
        {
            var stringBuilder = new StringBuilder();
            Serialize(input, stringBuilder, 0);
            return stringBuilder.ToString();
        }

        private static void Serialize(
            object input, 
            StringBuilder stringBuilder,
            int indentLevel,
            bool forceClassName = false)
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
                        throw new HatchetException(
                            $"`{keyStr}` is an invalid dictionary key. Key cannot contain spaces.");
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

            if (inputType.GenericTypeArguments.Length == 1)
            {
                var elementType = inputType.GenericTypeArguments[0];

                if (elementType.IsAbstract)
                    forceClassName = true;

                var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

                stringBuilder.Append("[");

                if (enumerableType.IsAssignableFrom(inputType))
                {
                    var enr = ((IEnumerable) input).GetEnumerator();

                    var addSpace = false;
                    while (enr.MoveNext())
                    {
                        if (addSpace)
                            stringBuilder.AppendFormat(" ");
                        addSpace = true;

                        var o = enr.Current;
                        Serialize(o, stringBuilder, indentLevel + 1, forceClassName);
                    }
                }

                stringBuilder.Append("]");
                return;
            }

            if (typeof (ICollection).IsAssignableFrom(inputType))
            {
                var inputList = (ICollection) input;

                foreach (var item in inputList)
                {
                    Serialize(item, stringBuilder, indentLevel + 1, forceClassName);
                }

                return;
            }

            if (inputType.IsPrimitive 
                || inputType.IsEnum
                || inputType == typeof(decimal) 
                || inputType == typeof(DateTime)
                || inputType == typeof(Guid))
            {
                stringBuilder.Append(input);
                return;
            }

            if (inputType.IsClass || inputType.IsValueType)
            {
                SerializeClassOrStruct(input, stringBuilder, indentLevel, inputType, forceClassName);
                return;
            }

            throw new HatchetException($"Could not serialize {input} of type {inputType}");
        }

        private static void SerializeClassOrStruct(
            object input, 
            StringBuilder stringBuilder, 
            int indentLevel, 
            Type inputType, 
            bool forceClassName)
        {
            stringBuilder.Append("{");
            stringBuilder.Append(LineEnding);

            if (forceClassName)
            {
                stringBuilder.Append(' ', indentLevel * IndentCount);
                stringBuilder.Append(' ', IndentCount);
                stringBuilder.AppendFormat("Class {0}", inputType.Name);
                stringBuilder.Append(LineEnding);
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var field in inputType.GetFields(bindingFlags))
            {
                if (field.HasAttribute<HatchetIgnoreAttribute>())
                    continue;

                var keyStr = field.Name;
                var value = field.GetValue(input);

                if (value == null)
                    continue;

                var fcn = field.FieldType.IsAbstract;

                stringBuilder.Append(' ', indentLevel * IndentCount);
                stringBuilder.Append(' ', IndentCount);
                stringBuilder.Append(keyStr);
                stringBuilder.Append(' ');
                Serialize(value, stringBuilder, indentLevel + 1, fcn);
                stringBuilder.Append(LineEnding);
            }

            foreach (var property in inputType.GetProperties(bindingFlags))
            {
                if (property.HasAttribute<HatchetIgnoreAttribute>())
                    continue;

                if (property.SetMethod == null)
                    continue;

                var keyStr = property.Name;
                var value = property.GetValue(input);

                if (value == null)
                    continue;

                var fcn = property.PropertyType.IsAbstract;

                stringBuilder.Append(' ', indentLevel * IndentCount);
                stringBuilder.Append(' ', IndentCount);
                stringBuilder.Append(keyStr);
                stringBuilder.Append(' ');
                Serialize(value, stringBuilder, indentLevel + 1, fcn);
                stringBuilder.Append(LineEnding);
            }

            stringBuilder.Append(' ', indentLevel * IndentCount);
            stringBuilder.Append("}");
        }
    }
}