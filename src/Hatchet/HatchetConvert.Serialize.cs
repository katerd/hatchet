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
                SerializeString(stringBuilder, inputAsString);
                return;
            }
            
            if (input is DateTime)
            {
                SerializeDateTime(input, stringBuilder);
                return;
            }

            var inputType = input.GetType();
            if (inputType.IsArray)
            {
                SerializeArray(input, stringBuilder);
                return;
            }

            var inputDictionary = input as IDictionary;
            if (inputDictionary != null)
            {
                SerializeDictionary(stringBuilder, indentLevel, inputDictionary);
                return;
            }

            if (inputType.GenericTypeArguments.Length == 1)
            {
                SerializeGenericEnumerable(input, stringBuilder, indentLevel, forceClassName, inputType);
                return;
            }

            if (typeof (ICollection).IsAssignableFrom(inputType))
            {
                SerializeCollection(input, stringBuilder, indentLevel, forceClassName);
                return;
            }

            if (IsSimpleValue(inputType))
            {
                stringBuilder.Append(input);
            }
            else if (inputType.IsEnum)
            {
                SerializeEnum(input, stringBuilder);
            }
            else if (inputType.IsClass || inputType.IsValueType)
            {
                SerializeClassOrStruct(input, stringBuilder, indentLevel, inputType, forceClassName);
            }
            else
            {
                throw new HatchetException($"Could not serialize {input} of type {inputType}");
            }
        }

        private static bool IsSimpleValue(Type inputType)
        {
            return inputType.IsPrimitive 
                   || inputType == typeof(decimal) 
                   || inputType == typeof(DateTime)
                   || inputType == typeof(Guid);
        }

        private static void SerializeEnum(object input, StringBuilder stringBuilder)
        {
            var strRepr = input.ToString();
            if (strRepr.IndexOf(',') > 0)
            {
                stringBuilder.AppendFormat("[{0}]", strRepr);
            }
            else
            {
                stringBuilder.Append(strRepr);
            }
        }

        private static void SerializeDateTime(object input, StringBuilder stringBuilder)
        {
            var inputAsDateTime = (DateTime) input;
            stringBuilder.AppendFormat("\"{0:O}\"", inputAsDateTime);
        }

        private static void SerializeArray(object input, StringBuilder stringBuilder)
        {
            var inputArray = (Array) input;
            stringBuilder.AppendFormat("[{0}]", string.Join(" ", inputArray.Select(Serialize)));
        }

        private static void SerializeCollection(object input, StringBuilder stringBuilder, int indentLevel, bool forceClassName)
        {
            var inputList = (ICollection) input;

            foreach (var item in inputList)
            {
                Serialize(item, stringBuilder, indentLevel + 1, forceClassName);
            }
        }

        private static void SerializeGenericEnumerable(object input, StringBuilder stringBuilder, int indentLevel, bool forceClassName,
            Type inputType)
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
        }

        private static void SerializeString(StringBuilder stringBuilder, string inputAsString)
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
        }

        private static void SerializeDictionary(StringBuilder stringBuilder, int indentLevel, IDictionary inputDictionary)
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

                SerializeKeyValue(stringBuilder, indentLevel, keyStr, value);
            }

            stringBuilder.Append(' ', indentLevel * IndentCount);
            stringBuilder.Append("}");
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
                SerializeField(input, stringBuilder, indentLevel, field);
            }

            foreach (var property in inputType.GetProperties(bindingFlags))
            {
                SerializeProperty(input, stringBuilder, indentLevel, property);
            }

            stringBuilder.Append(' ', indentLevel * IndentCount);
            stringBuilder.Append("}");
        }

        private static void SerializeProperty(object input, StringBuilder stringBuilder, int indentLevel, PropertyInfo property)
        {
            if (property.HasAttribute<HatchetIgnoreAttribute>())
                return;

            if (property.SetMethod == null)
                return;

            var keyStr = property.Name;
            var value = property.GetValue(input);

            if (value == null)
                return;

            var forceClassName = property.PropertyType.IsAbstract;

            SerializeKeyValue(stringBuilder, indentLevel, keyStr, value, forceClassName);
        }

        private static void SerializeField(object input, StringBuilder stringBuilder, int indentLevel, FieldInfo field)
        {
            if (field.HasAttribute<HatchetIgnoreAttribute>())
                return;

            var keyStr = field.Name;
            var value = field.GetValue(input);

            if (value == null)
                return;

            var forceClassName = field.FieldType.IsAbstract;

            SerializeKeyValue(stringBuilder, indentLevel, keyStr, value, forceClassName);
        }
        
        private static void SerializeKeyValue(StringBuilder stringBuilder, int indentLevel, string key, object value, bool forceClassName = false)
        {
            stringBuilder.Append(' ', indentLevel * IndentCount);
            stringBuilder.Append(' ', IndentCount);
            stringBuilder.Append(key);
            stringBuilder.Append(' ');
            Serialize(value, stringBuilder, indentLevel + 1, forceClassName);
            stringBuilder.Append(LineEnding);
        }
    }
}