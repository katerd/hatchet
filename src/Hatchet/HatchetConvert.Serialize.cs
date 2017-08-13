using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Hatchet.Extensions;
using Hatchet.Reflection;

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
            Serialize(input, prettyPrinter);
            return stringBuilder.ToString();
        }
        
        private static void Serialize(
            object input, 
            PrettyPrinter prettyPrinter,
            bool forceClassName = false)
        {
            foreach (var conversionFunction in SerializationRules)
            {
                if (conversionFunction.Item1(input))
                {
                    conversionFunction.Item2(input, prettyPrinter, forceClassName);
                    return;
                }
            }
            throw new HatchetException($"Could not serialize {input} of type {input.GetType()}");
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

        private static void SerializeCollection(object input, PrettyPrinter prettyPrinter, bool forceClassName)
        {
            var inputList = (ICollection) input;

            foreach (var item in inputList)
            {
                prettyPrinter.Indent();
                Serialize(item, prettyPrinter, forceClassName);
                prettyPrinter.Deindent();
            }
        }

        private static void SerializeGenericEnumerable(object input, PrettyPrinter prettyPrinter, bool forceClassName)
        {
            var inputType = input.GetType();
            
            var elementType = inputType.GenericTypeArguments[0];

            if (elementType.IsAbstract)
                forceClassName = true;

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            prettyPrinter.Append("[");

            if (enumerableType.IsAssignableFrom(inputType))
            {
                var enumerator = ((IEnumerable) input).GetEnumerator();

                var addSpace = false;
                while (enumerator.MoveNext())
                {
                    if (addSpace)
                        prettyPrinter.AppendFormat(" ");
                    addSpace = true;

                    var element = enumerator.Current;
                    prettyPrinter.Indent();
                    Serialize(element, prettyPrinter, forceClassName);
                    prettyPrinter.Deindent();
                }
            }

            prettyPrinter.Append("]");
        }

        private static void SerializeDictionary(PrettyPrinter prettyPrinter, IDictionary inputDictionary)
        {
            if (inputDictionary.Count == 0)
            {
                prettyPrinter.Append("{}");
                return;
            }
            
            prettyPrinter.AppendOpenBlock();

            foreach (var key in inputDictionary.Keys)
            {
                var keyStr = key.ToString();
                if (keyStr.Contains(" "))
                {
                    throw new HatchetException(
                        $"`{keyStr}` is an invalid dictionary key. Key cannot contain spaces.");
                }

                var value = inputDictionary[keyStr];

                SerializeKeyValue(prettyPrinter, keyStr, value);
            }

            prettyPrinter.AppendCloseBlock();
        }

        private static void SerializeClassOrStruct(object input, PrettyPrinter prettyPrinter, bool forceClassName)
        {
            var inputType = input.GetType();
            
            prettyPrinter.AppendOpenBlock();

            if (forceClassName)
            {
                WriteClassName(prettyPrinter, inputType);
            }

            SerializeFieldsAndProperties(input, prettyPrinter, inputType);

            prettyPrinter.AppendCloseBlock();
        }

        private static void SerializeFieldsAndProperties(object input, PrettyPrinter prettyPrinter, Type inputType)
        {
            var propertiesAndFields = GetPropertiesAndFields(input, inputType);

            foreach (var member in propertiesAndFields)
            {
                SerializeMember(prettyPrinter, member);
            }
        }

        private static IEnumerable<ISerializableMember> GetPropertiesAndFields(object input, Type inputType)
        {
            foreach (var property in inputType.GetPropertiesToSerialize())
            {
                yield return new SerializableProperty(property, input);
            }
            
            foreach (var field in inputType.GetFieldsToSerialize())
            {
                yield return new SerializableField(field, input);
            }
        }

        private static void WriteClassName(PrettyPrinter prettyPrinter, Type inputType)
        {
            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append(' ', IndentCount);
            prettyPrinter.AppendFormat("Class {0}", inputType.Name);
            prettyPrinter.Append(LineEnding);
        }

        private static void SerializeMember(PrettyPrinter prettyPrinter, ISerializableMember member)
        {
            SerializeKeyValue(prettyPrinter, member.Name, member.Value, member.IsValueAbstract);
        }
        
        private static void SerializeKeyValue(PrettyPrinter prettyPrinter, string key, object value, bool forceClassName = false)
        {
            if (value == null)
                return;
            
            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append(' ', IndentCount);
            prettyPrinter.Append(key);
            prettyPrinter.Append(' ');
            prettyPrinter.Indent();
            Serialize(value, prettyPrinter, forceClassName);
            prettyPrinter.Deindent();
            prettyPrinter.Append(LineEnding);
        }
    }
}