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
                    var context = new SerializationContext(input, prettyPrinter, forceClassName);
                    
                    conversionFunction.Item2(context);
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

        private static void SerializeCollection(SerializationContext context)
        {
            var input = context.Input;
            var prettyPrinter = context.Printer;
            var forceClassName = context.ForceClassName;
            
            var inputList = (ICollection) input;

            foreach (var item in inputList)
            {
                IndentAndSerialize(prettyPrinter, item, forceClassName);
            }
        }

        private static void SerializeGenericEnumerable(SerializationContext context)
        {
            var input = context.Input;
            var prettyPrinter = context.Printer;
            var forceClassName = context.ForceClassName;
            
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
                    IndentAndSerialize(prettyPrinter, element, forceClassName);
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

            using (prettyPrinter.StartParenBlock())
            {
                foreach (var key in inputDictionary.Keys)
                {
                    SerializeKeyValue(prettyPrinter, key.ToString(), inputDictionary[key]);
                }
            }
        }

        private static void SerializeClassOrStruct(SerializationContext context)
        {
            var input = context.Input;
            var prettyPrinter = context.Printer;

            var inputType = input.GetType();

            using (prettyPrinter.StartParenBlock())
            {
                if (context.ForceClassName)
                {
                    WriteClassName(prettyPrinter, inputType);
                }
                SerializeFieldsAndProperties(input, prettyPrinter, inputType);
            }
        }

        private static void SerializeFieldsAndProperties(object input, PrettyPrinter prettyPrinter, Type inputType)
        {
            var propertiesAndFields = GetPropertiesAndFields(input, inputType);

            foreach (var member in propertiesAndFields)
            {
                SerializeMember(prettyPrinter, member);
            }
        }

        private static void SerializeMember(PrettyPrinter prettyPrinter, ISerializableMember member)
        {
            SerializeKeyValue(prettyPrinter, member.Name, member.Value, member.IsValueAbstract);
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

        private static void SerializeKeyValue(PrettyPrinter prettyPrinter, string key, object value, bool forceClassName = false)
        {
            if (value == null)
                return;
            
            if (key.Contains(" "))
            {
                throw new HatchetException(
                    $"`{key}` is an invalid key. Key cannot contain spaces.");
            }
            
            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append(' ', IndentCount);
            prettyPrinter.Append(key);
            prettyPrinter.Append(' ');
            IndentAndSerialize(prettyPrinter, value, forceClassName);
            prettyPrinter.Append(LineEnding);
        }

        private static void IndentAndSerialize(PrettyPrinter prettyPrinter, object value, bool forceClassName)
        {
            using (prettyPrinter.StartIndent())
            {
                Serialize(value, prettyPrinter, forceClassName);
            }
        }
    }
}