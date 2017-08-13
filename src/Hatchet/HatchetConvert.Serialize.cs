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
            var prettyPrinter = new PrettyPrinter(stringBuilder);
            Serialize(input, prettyPrinter, 0);
            return stringBuilder.ToString();
        }

        private static void Serialize(
            object input, 
            PrettyPrinter prettyPrinter,
            int indentLevel,
            bool forceClassName = false)
        {
            var inputAsString = input as string;
            if (inputAsString != null)
            {
                prettyPrinter.AppendString(inputAsString);
                return;
            }
            
            if (input is DateTime)
            {
                prettyPrinter.AppendDateTime(input);
                return;
            }

            var inputType = input.GetType();
            if (inputType.IsArray)
            {
                SerializeArray(input, prettyPrinter);
                return;
            }

            var inputDictionary = input as IDictionary;
            if (inputDictionary != null)
            {
                SerializeDictionary(prettyPrinter, indentLevel, inputDictionary);
                return;
            }

            if (inputType.GenericTypeArguments.Length == 1)
            {
                SerializeGenericEnumerable(input, prettyPrinter, indentLevel, forceClassName, inputType);
                return;
            }

            if (typeof (ICollection).IsAssignableFrom(inputType))
            {
                SerializeCollection(input, prettyPrinter, indentLevel, forceClassName);
                return;
            }

            if (IsSimpleValue(inputType))
            {
                prettyPrinter.Append(input);
            }
            else if (inputType.IsEnum)
            {
                prettyPrinter.AppendEnum(input);
            }
            else if (inputType.IsClass || inputType.IsValueType)
            {
                SerializeClassOrStruct(input, prettyPrinter, indentLevel, inputType, forceClassName);
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

        private static void SerializeArray(object input, PrettyPrinter stringBuilder)
        {
            var inputArray = (Array) input;
            stringBuilder.AppendFormat("[{0}]", string.Join(" ", inputArray.Select(Serialize)));
        }

        private static void SerializeCollection(object input, PrettyPrinter prettyPrinter, int indentLevel, bool forceClassName)
        {
            var inputList = (ICollection) input;

            foreach (var item in inputList)
            {
                Serialize(item, prettyPrinter, indentLevel + 1, forceClassName);
            }
        }

        private static void SerializeGenericEnumerable(object input, PrettyPrinter prettyPrinter, int indentLevel, bool forceClassName,
            Type inputType)
        {
            var elementType = inputType.GenericTypeArguments[0];

            if (elementType.IsAbstract)
                forceClassName = true;

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            prettyPrinter.Append("[");

            if (enumerableType.IsAssignableFrom(inputType))
            {
                var enr = ((IEnumerable) input).GetEnumerator();

                var addSpace = false;
                while (enr.MoveNext())
                {
                    if (addSpace)
                        prettyPrinter.AppendFormat(" ");
                    addSpace = true;

                    var o = enr.Current;
                    Serialize(o, prettyPrinter, indentLevel + 1, forceClassName);
                }
            }

            prettyPrinter.Append("]");
        }

        private static void SerializeDictionary(PrettyPrinter prettyPrinter, int indentLevel, IDictionary inputDictionary)
        {
            if (inputDictionary.Count == 0)
            {
                prettyPrinter.Append("{}");
                return;
            }

            prettyPrinter.Append("{");
            prettyPrinter.Append(LineEnding);

            foreach (var key in inputDictionary.Keys)
            {
                var keyStr = key.ToString();
                if (keyStr.Contains(" "))
                {
                    throw new HatchetException(
                        $"`{keyStr}` is an invalid dictionary key. Key cannot contain spaces.");
                }

                var value = inputDictionary[keyStr];

                SerializeKeyValue(prettyPrinter, indentLevel, keyStr, value);
            }

            prettyPrinter.Append(' ', indentLevel * IndentCount);
            prettyPrinter.Append("}");
        }

        private static void SerializeClassOrStruct(
            object input, 
            PrettyPrinter prettyPrinter, 
            int indentLevel, 
            Type inputType, 
            bool forceClassName)
        {
            prettyPrinter.Append("{");
            prettyPrinter.Append(LineEnding);

            if (forceClassName)
            {
                prettyPrinter.Append(' ', indentLevel * IndentCount);
                prettyPrinter.Append(' ', IndentCount);
                prettyPrinter.AppendFormat("Class {0}", inputType.Name);
                prettyPrinter.Append(LineEnding);
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var field in inputType.GetFields(bindingFlags))
            {
                SerializeField(input, prettyPrinter, indentLevel, field);
            }

            foreach (var property in inputType.GetProperties(bindingFlags))
            {
                SerializeProperty(input, prettyPrinter, indentLevel, property);
            }

            prettyPrinter.Append(' ', indentLevel * IndentCount);
            prettyPrinter.Append("}");
        }

        private static void SerializeProperty(object input, PrettyPrinter prettyPrinter, int indentLevel, PropertyInfo property)
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

            SerializeKeyValue(prettyPrinter, indentLevel, keyStr, value, forceClassName);
        }

        private static void SerializeField(object input, PrettyPrinter prettyPrinter, int indentLevel, FieldInfo field)
        {
            if (field.HasAttribute<HatchetIgnoreAttribute>())
                return;

            var keyStr = field.Name;
            var value = field.GetValue(input);

            if (value == null)
                return;

            var forceClassName = field.FieldType.IsAbstract;

            SerializeKeyValue(prettyPrinter, indentLevel, keyStr, value, forceClassName);
        }
        
        private static void SerializeKeyValue(PrettyPrinter prettyPrinter, int indentLevel, string key, object value, bool forceClassName = false)
        {
            prettyPrinter.Append(' ', indentLevel * IndentCount);
            prettyPrinter.Append(' ', IndentCount);
            prettyPrinter.Append(key);
            prettyPrinter.Append(' ');
            Serialize(value, prettyPrinter, indentLevel + 1, forceClassName);
            prettyPrinter.Append(LineEnding);
        }
    }
}