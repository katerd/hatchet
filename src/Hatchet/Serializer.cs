using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hatchet.Extensions;
using Hatchet.Reflection;

namespace Hatchet;

internal class Serializer(PrettyPrinter prettyPrinter, SerializeOptions serializeOptions)
{
    private const string LineEnding = "\n";
    private const int IndentCount = 2;
        
    private PrettyPrinter PrettyPrinter { get; } = prettyPrinter;
    private SerializeOptions SerializeOptions { get; } = serializeOptions;

    private int IndentLevel => PrettyPrinter.IndentLevel;
        
    private readonly List<object> _metObjects = [];

    public void Serialize(object input, bool forceClassName = false)
    {
        PushObjectRef(input);
            
        var context = new SerializationContext(forceClassName);

        switch (input)
        {
            case Array arrayInput when arrayInput.GetType().IsArray:
                SerializeArray(arrayInput);
                break;
            case IDictionary dictionaryInput:
                SerializeDictionary(dictionaryInput);
                break;
            case not null when input.GetType().GenericTypeArguments.Length == 1:
                SerializeGenericEnumerable(input, context);
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
            case not null when IsSimpleValue(input.GetType()):
                SerializeSimpleValue(input);
                break;
            case ICollection collectionInput:
                SerializeCollection(collectionInput, context);
                break;
            case Enum enumValue when enumValue.GetType().IsEnum:
                SerializeEnum(enumValue);
                break;
            case not null when input.GetType().IsClass || input.GetType().IsValueType:
                SerializeClassOrStruct(input, context);
                break;
            default:
                throw new HatchetException($"Could not serialize {input} of type {input?.GetType()}");
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
        var inputType = input.GetType();

        var customOutputValue = inputType
            .GetNonIgnoredProperties()
            .SingleOrDefault(x => x.HasAttribute<HatchetValueAttribute>());

        if (customOutputValue != null)
        {
            var value = customOutputValue.GetValue(input);
            PrettyPrinter.Append(value);
            return;
        }

        PrettyPrinter.AppendOpenBlock();
        if (context.ForceClassName)
        {
            WriteClassName(inputType);
        }
        SerializeFieldsAndProperties(input);
        PrettyPrinter.AppendCloseBlock();
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
        PrettyPrinter.AppendFormat("Class {0}", inputType.Name);
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
        Serialize(value, forceClassName);
        PrettyPrinter.Unindent();
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
        
    private void SerializeArray(Array inputArray)
    {
        var values = inputArray.Select(x => HatchetConvert.Serialize(x, SerializeOptions));

        PrettyPrinter.AppendFormat("[{0}]", string.Join(" ", values));
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
