using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatchet.Extensions;
using Hatchet.Reflection;

namespace Hatchet
{
    internal class Serializer
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;
        
        private PrettyPrinter PrettyPrinter { get; }
        private StringBuilder StringBuilder { get; }
        private SerializeOptions SerializeOptions { get; }

        private int IndentLevel => PrettyPrinter.IndentLevel;
        
        private readonly List<object> _metObjects;

        public Serializer(
            PrettyPrinter prettyPrinter, 
            StringBuilder stringBuilder, 
            SerializeOptions serializeOptions)
        {
            PrettyPrinter = prettyPrinter;
            StringBuilder = stringBuilder;
            SerializeOptions = serializeOptions;
            _metObjects = new List<object>();
        }

        internal void StaticSerialize(object input, bool forceClassName = false)
        {
            PushObjectRef(input);
            
            var context = new SerializationContext(this, forceClassName);

            switch (input)
            {
                case Array arrayInput when arrayInput.GetType().IsArray:
                    SerializeArray(arrayInput, context);
                    break;
                case IDictionary dictionaryInput:
                    SerializeDictionary(dictionaryInput);
                    break;
                case object genericEnumerable when genericEnumerable.GetType().GenericTypeArguments.Length == 1:
                    SerializeGenericEnumerable(genericEnumerable, context);
                    break;
                case string strInput:
                    SerializeString(strInput);
                    break;
                case DateTime dateTimeInput:
                    SerializeDateTime(dateTimeInput);
                    break;
                case bool boolInput:
                    SerializeBoolean(boolInput);
                    break;
                case object simpleValue when IsSimpleValue(simpleValue.GetType()):
                    SerializeSimpleValue(simpleValue);
                    break;
                case ICollection collectionInput:
                    SerializeCollection(collectionInput, context);
                    break;
                case Enum enumValue when enumValue.GetType().IsEnum:
                    SerializeEnum(enumValue);
                    break;
                case object classOrStruct when classOrStruct.GetType().IsClass || classOrStruct.GetType().IsValueType:
                    SerializeClassOrStruct(classOrStruct, context);
                    break;
                default:
                    throw new HatchetException($"Could not serialize {input} of type {input.GetType()}");
            }
            
            PopObjectRef(input);
        }
        
        private bool IsSimpleValue(Type inputType)
        {
            return inputType.IsPrimitive 
                   || inputType == typeof(decimal) 
                   || inputType == typeof(DateTime)
                   || inputType == typeof(Guid);
        }
        
        private void SerializeClassOrStruct(object input, SerializationContext context)
        {   
            var prettyPrinter = context.Serializer;

            var inputType = input.GetType();

            var customOutputValue = inputType
                .GetNonIgnoredProperties()
                .SingleOrDefault(x => x.HasAttribute<HatchetValueAttribute>());

            if (customOutputValue != null)
            {
                var value = customOutputValue.GetValue(input);
                prettyPrinter.PrettyPrinter.Append(value);
                return;
            }

            prettyPrinter.PrettyPrinter.AppendOpenBlock();
            if (context.ForceClassName)
            {
                WriteClassName(inputType);
            }
            SerializeFieldsAndProperties(input);
            prettyPrinter.PrettyPrinter.AppendCloseBlock();
        }
        
        private void SerializeFieldsAndProperties(object input)
        {
            var propertiesAndFields = GetPropertiesAndFields(input);

            foreach (var member in propertiesAndFields)
            {
                SerializeMember(member);
            }
        }
        
        private void SerializeMember(ISerializableMember member)
        {
            SerializeKeyValue(member.Name, member.Value, member.IsValueAbstract);
        }
        
        private void WriteClassName(Type inputType)
        {
            PrettyPrinter.Append(' ', IndentLevel * IndentCount);
            PrettyPrinter.Append(' ', IndentCount);
            PrettyPrinter.AppendFormat("Class {0}", new[] {inputType.Name});
            PrettyPrinter.Append(LineEnding);
        }

        private void SerializeKeyValue(string key, object value, bool forceClassName = false)
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
                if (value.Equals(comparable) && !SerializeOptions.IncludeDefaultValues)
                    return;
            }

            PrettyPrinter.Append(' ', IndentLevel * IndentCount);
            PrettyPrinter.Append(' ', IndentCount);
            PrettyPrinter.Append(key);
            PrettyPrinter.Append(' ');
            IndentAndSerialize(value, forceClassName);
            PrettyPrinter.Append(LineEnding);
        }

        private void IndentAndSerialize(object value, bool forceClassName)
        {
            PrettyPrinter.Indent();
            StaticSerialize(value, forceClassName);
            PrettyPrinter.Deindent();
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
        
        private void SerializeEnum(object value)
        {
             PrettyPrinter.AppendEnum(value);
        }
        
        private void SerializeSimpleValue(object input)
        {
            PrettyPrinter.Append(input);
        }
        
        private void SerializeGenericEnumerable(object input, SerializationContext context)
        {
            var forceClassName = context.ForceClassName;

            var elementType = input.GetType().GenericTypeArguments[0];

            if (elementType.IsAbstract)
                forceClassName = true;

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            PrettyPrinter.Append("[");

            if (enumerableType.IsInstanceOfType(input))
            {
                var enumerator = ((IEnumerable) input).GetEnumerator();

                var addSpace = false;
                while (enumerator.MoveNext())
                {
                    if (addSpace)
                        PrettyPrinter.AppendFormat(" ");
                    addSpace = true;

                    var element = enumerator.Current;
                    IndentAndSerialize(element, forceClassName);
                }
            }

            PrettyPrinter.Append("]");
        }
        
        private void SerializeDictionary(IDictionary input)
        {
            if (input.Count == 0)
            {
                PrettyPrinter.Append("{}");
                return;
            }

            PrettyPrinter.AppendOpenBlock();
            foreach (var key in input.Keys)
            {
                SerializeKeyValue(key.ToString(), input[key]);
            }
            PrettyPrinter.AppendCloseBlock();
        }
        
        private void SerializeCollection(IEnumerable collectionInput, SerializationContext context)
        {
            var forceClassName = context.ForceClassName;

            foreach (var item in collectionInput)
            {
                IndentAndSerialize(item, forceClassName);
            }
        }
        
        private void SerializeArray(Array inputArray, SerializationContext context)
        {
            var values = inputArray.Select(x => HatchetConvert.Serialize(x, context.Serializer.SerializeOptions));

            PrettyPrinter.AppendFormat("[{0}]", new[] {string.Join(" ", values)});
        }

        private void SerializeString(string input)
        {
            PrettyPrinter.AppendString(input);
        }

        private void SerializeDateTime(DateTime input)
        {
            PrettyPrinter.AppendDateTime(input);
        }

        private void SerializeBoolean(bool input)
        {
            PrettyPrinter.Append(input ? "true" : "false");
        }

        private void PushObjectRef(object obj)
        {
            var type = obj.GetType();

            if (obj is string)
                return;
            
            if (type.IsValueType)
                return;
            
            if (_metObjects.Contains(obj))
                throw new CircularReferenceException(obj);
            _metObjects.Add(obj);
        }

        private void PopObjectRef(object obj)
        {
            _metObjects.Remove(obj);
        }
    }
}