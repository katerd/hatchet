using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            return Serialize(input, new SerializeOptions());
        }
        
        public static string Serialize(object input, SerializeOptions serializeOptions)
        {
            var stringBuilder = new StringBuilder();
            var prettyPrinter = new PrettyPrinter(stringBuilder);
            var serializer = new Serializer(prettyPrinter, stringBuilder, serializeOptions);

            Serialize(input, serializer);
            return stringBuilder.ToString();
        }
        
        private static void Serialize(
            object input, 
            Serializer serializer,
            bool forceClassName = false)
        {
            serializer.PushObjectRef(input);
            
            var context = new SerializationContext(input, serializer, forceClassName);
            
            foreach (var conversionFunction in SerializationRules)
            {
                if (conversionFunction.Item1(input))
                {
                    conversionFunction.Item2(context);
                    serializer.PopObjectRef(input);
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

        private static void SerializeArray(SerializationContext context)
        {
            var inputArray = (Array) context.Input;
            context.Printer.AppendFormat("[{0}]", string.Join(" ", inputArray.Select(Serialize)));
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

            var elementType = input.GetType().GenericTypeArguments[0];

            if (elementType.IsAbstract)
                forceClassName = true;

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            prettyPrinter.Append("[");

            if (enumerableType.IsInstanceOfType(input))
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

        private static void SerializeDictionary(SerializationContext context)
        {
            var inputDictionary = (IDictionary) context.Input;
            var prettyPrinter = context.Printer;

            if (inputDictionary.Count == 0)
            {
                prettyPrinter.Append("{}");
                return;
            }

            prettyPrinter.AppendOpenBlock();
            foreach (var key in inputDictionary.Keys)
            {
                SerializeKeyValue(prettyPrinter, key.ToString(), inputDictionary[key]);
            }
            prettyPrinter.AppendCloseBlock();
        }

        private static void SerializeClassOrStruct(SerializationContext context)
        {   
            var input = context.Input;
            var prettyPrinter = context.Printer;

            var inputType = input.GetType();

            var customOutputValue = inputType
                .GetNonIgnoredProperties()
                .SingleOrDefault(x => x.HasAttribute<HatchetValueAttribute>());

            if (customOutputValue != null)
            {
                var value = customOutputValue.GetValue(input);
                prettyPrinter.Append(value);
                return;
            }
            
            prettyPrinter.AppendOpenBlock();
            if (context.ForceClassName)
            {
                WriteClassName(prettyPrinter, inputType);
            }
            SerializeFieldsAndProperties(context);
            prettyPrinter.AppendCloseBlock();
        }

        private static void SerializeFieldsAndProperties(SerializationContext context)
        {
            var propertiesAndFields = GetPropertiesAndFields(context.Input);

            foreach (var member in propertiesAndFields)
            {
                SerializeMember(context.Printer, member);
            }
        }

        private static void SerializeMember(Serializer serializer, ISerializableMember member)
        {
            SerializeKeyValue(serializer, member.Name, member.Value, member.IsValueAbstract);
        }

        private static IEnumerable<ISerializableMember> GetPropertiesAndFields(object input)
        {
            var inputType = input.GetType();
            
            foreach (var property in inputType.GetPropertiesToSerialize())
            {
                yield return new SerializableProperty(property, input);
            }
            
            foreach (var field in inputType.GetFieldsToSerialize())
            {
                yield return new SerializableField(field, input);
            }
        }

        private static void WriteClassName(Serializer serializer, Type inputType)
        {
            serializer.Append(' ', serializer.IndentLevel * IndentCount);
            serializer.Append(' ', IndentCount);
            serializer.AppendFormat("Class {0}", inputType.Name);
            serializer.Append(LineEnding);
        }

        private static void SerializeKeyValue(Serializer serializer, string key, object value, bool forceClassName = false)
        {
            if (value == null)
                return;
            
            if (key.Contains(" "))
            {
                throw new HatchetException(
                    $"`{key}` is an invalid key. Key cannot contain spaces.");
            }
            
            var type = value.GetType();
            
            if (type.IsValueType)
            {
                var comparable = Activator.CreateInstance(type);
                if (value.Equals(comparable) && !serializer.SerializeOptions.IncludeDefaultValues)
                    return;
            }
            
            serializer.Append(' ', serializer.IndentLevel * IndentCount);
            serializer.Append(' ', IndentCount);
            serializer.Append(key);
            serializer.Append(' ');
            IndentAndSerialize(serializer, value, forceClassName);
            serializer.Append(LineEnding);
        }

        private static void IndentAndSerialize(Serializer serializer, object value, bool forceClassName)
        {
            serializer.Indent();
            Serialize(value, serializer, forceClassName);
            serializer.Deindent();
        }

        private static void SerializeBoolean(SerializationContext context)
        {
            context.Printer.Append((bool)context.Input ? "true" : "false");
        }
    }
}