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

            Serializer.Serialize(input, serializer);
            return stringBuilder.ToString();
        }
        
        private static void SerializeClassOrStruct(SerializationContext context)
        {   
            var input = context.Input;
            var prettyPrinter = context.Serializer;

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
                SerializeMember(context.Serializer, member);
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

        internal static void SerializeKeyValue(Serializer serializer, string key, object value, bool forceClassName = false)
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

        internal static void IndentAndSerialize(Serializer serializer, object value, bool forceClassName)
        {
            serializer.Indent();
            Serializer.Serialize(value, serializer, forceClassName);
            serializer.Deindent();
        }
    }
}