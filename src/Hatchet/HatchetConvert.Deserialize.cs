using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hatchet.Extensions;

namespace Hatchet;

public static partial class HatchetConvert
{
    internal const string ClassNameKey = "Class";

    public static T? Deserialize<T>(string input)
    {
        var parser = new Parser();
        var result = parser.Parse(ref input);
        var type = typeof(T);
        if (result == null)
        {
            return default;
        }

        return (T?)DeserializeObject(result, type);
    }

    private static object? DeserializeObject(object result, Type type)
    {
        var context = new DeserializationContext(result, type);

        var count = DeserializationRules.Count;
        for (var index = 0; index < count; index++)
        {
            var rule = DeserializationRules[index];
            if (rule.Item1(context))
            {
                return rule.Item2(context);
            }
        }

        throw new HatchetException($"Unable to convert {result} - unknown type {type}");
    }

    private static bool IsGenericCollection(object result, Type type)
    {
        return result is ICollection && type.IsGenericType;
    }

    private static bool IsNullableValueType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    private static bool IsComplexType(Type type)
    {
        return type.IsClass || type.IsValueType || type.IsInterface;
    }

    private static object? DeserializeNullableValueType(DeserializationContext context)
    {
        var input = context.Input;
        var type = context.OutputType;
            
        if (input.ToString().Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;

        var actualValue = Convert.ChangeType(input, type.GenericTypeArguments[0]);
        var nullableValue = Activator.CreateInstance(type, actualValue);
        return nullableValue;
    }

    private static object DeserializeGenericCollection(DeserializationContext context)
    {
        var type = context.OutputType;
        var input = context.Input;
            
        var elementType = type.GenericTypeArguments[0];

        var setG = typeof(ISet<>).MakeGenericType(elementType);

        var inputList = (List<object>) input;

        if (setG.IsAssignableFrom(type))
        {
            return DeserializeHashSet(elementType, inputList);
        }

        var listG = typeof(List<>).MakeGenericType(elementType);
        if (type.IsAssignableFrom(listG))
        {
            return DeserializeList(elementType, inputList);
        }

        throw new HatchetException("Unable to deserialize generic collection");
    }

    private static object DeserializeList(Type elementType, List<object> inputList)
    {
        var genericListType = typeof(List<>).MakeGenericType(elementType);
        var outputList = (IList) Activator.CreateInstance(genericListType);

        foreach (var inputItem in inputList)
        {
            outputList.Add(DeserializeObject(inputItem, elementType));
        }

        return outputList;
    }

    private static object DeserializeHashSet(Type elementType, List<object> inputList)
    {
        var genericListType = typeof(HashSet<>).MakeGenericType(elementType);
        var outputList = Activator.CreateInstance(genericListType);

        foreach (var inputItem in inputList)
        {
            genericListType.InvokeMember(
                "Add",
                BindingFlags.InvokeMethod,
                null,
                outputList,
                [DeserializeObject(inputItem, elementType)]);
        }

        return outputList;
    }

    private static bool IsSimpleValueType(Type type) =>
        type.IsPrimitive
        || type == typeof(decimal)
        || type == typeof(DateTime);

    private static object DeserializeEnum(DeserializationContext context)
    {
        var input = context.Input;
        var type = context.OutputType;

        if (input is ICollection rItems)
        {
            if (rItems.Count == 0)
                return 0;

            var items = rItems.Select(x => x.ToString());
            var enumStr = string.Join(",", items);
            return Enum.Parse(type, enumStr, true);
        }

        return Enum.Parse(type, (string) input, true);
    }

    private static object DeserializeDictionary(DeserializationContext context)
    {
        var inputDictionary = (IDictionary) context.Input;
        var outputDictionary = (IDictionary) Activator.CreateInstance(context.OutputType);

        var outputGta = outputDictionary.GetType().GetGenericArguments();
        var outputKeyType = outputGta[0];
        var outputValueType = outputGta[1];

        // todo: skip this process if the input and output dictionary generic types match

        // go through each input dictionary key and convert to the output key and value.
        foreach (var key in inputDictionary.Keys)
        {
            var newKeyValue = DeserializeObject(key, outputKeyType);
            if (newKeyValue == null)
            {
                throw new Exception("Dictionary key deserialized to null");
            }

            var value = inputDictionary[key];
            var newValue = DeserializeObject(value, outputValueType);

            outputDictionary[newKeyValue] = newValue;
        }

        return outputDictionary;
    }

    private static object DeserializeArray(DeserializationContext context)
    {
        var arrayType = context.OutputType.GetElementType();
        if (arrayType == null)
        {
            throw new HatchetException("Unable to determine array element type");
        }
        
        var inputList = (List<object>) context.Input;
        var outputArray = Array.CreateInstance(arrayType, inputList.Count);

        for (var i = 0; i < inputList.Count; i++)
        {
            outputArray.SetValue(DeserializeObject(inputList[i], arrayType), i);
        }

        return outputArray;
    }

    private static object DeserializeComplexType(DeserializationContext context)
    {
        var input = context.Input;
        var type = context.OutputType;
            
        if (input is string)
        {
            var ctorMethod = ObjectFactory.FindStaticConstructorMethodWithSingleStringParameter(type);

            if (ctorMethod != null)
            {
                return ctorMethod.Invoke(null, [input]);
            }
                
            var ctor = FindConstructorWithSingleStringParameter(type);

            if (ctor != null)
            {
                return ctor.Invoke([input]);
            }

            throw new HatchetException($"Can't convert {input} to {type}"); 
        }

        var inputValues = (Dictionary<string, object>) input;

        var newtype = FindComplexType(type, inputValues);

        var instance = ObjectFactory.CreateComplexType(newtype, inputValues);
        SetComplexTypeFields(newtype, inputValues, instance);
        SetComplexTypeProperties(newtype, inputValues, instance);

        return instance;
    }

    private static object DeserializeGuid(DeserializationContext context) =>
        new Guid(context.Input.ToString());

    private static object DeserializeSimpleValue(DeserializationContext context) =>
        Convert.ChangeType(context.Input, context.OutputType);

    private static ConstructorInfo? FindConstructorWithSingleStringParameter(Type type) =>
        type.GetConstructors()
            .SingleOrDefault(x =>
            {
                var pc = x.GetParameters();
                if (pc.Length != 1)
                    return false;

                return pc[0].ParameterType == typeof(string);
            });

    private static Type FindComplexType(Type inType, Dictionary<string, object> inputValues)
    {
        if (!inputValues.TryGetValue(ClassNameKey, out var value))
        {
            return inType;
        }

        var name = value.ToString();
        var outType = HatchetTypeRegistry.GetType(name);
        return outType ??
               throw new HatchetException($"Can't create type - Type is not registered `{name}`");
    }

    private static void SetComplexTypeProperties(
        Type type, 
        Dictionary<string, object> inputValues, 
        object output)
    {
        var props = GetWritablePropertiesForType(type);

        var propsCount = props.Length;
        for (var index = 0; index < propsCount; index++)
        {
            var prop = props[index];
            var propName = prop.Name;
                
            if (!inputValues.TryGetValue(propName, out var value))
                continue;

            prop.SetValue(output, DeserializeObject(value, prop.PropertyType));
        }
    }

    private static readonly Dictionary<Type, PropertyInfo[]> PropertyInfoCache = new();

    private static PropertyInfo[] GetWritablePropertiesForType(Type type)
    {
        if (PropertyInfoCache.TryGetValue(type, out var propertyInfo))
            return propertyInfo;

        propertyInfo = type
            .GetProperties()
            .Where(x => x.CanWrite && !x.HasAttribute<HatchetIgnoreAttribute>())
            .ToArray();

        PropertyInfoCache[type] = propertyInfo;

        return propertyInfo;
    }

    private static readonly Dictionary<Type, FieldInfo[]> FieldInfoCache = new();

    private static FieldInfo[] GetFieldsForType(Type type)
    {
        if (FieldInfoCache.TryGetValue(type, out var fieldInfo))
            return fieldInfo;

        fieldInfo = type.GetFields()
            .Where(x => !x.HasAttribute<HatchetIgnoreAttribute>())
            .ToArray();

        FieldInfoCache[type] = fieldInfo;

        return fieldInfo;
    }
        
    private static void SetComplexTypeFields(
        Type type, 
        Dictionary<string, object> inputValues, 
        object output)
    {
        var fields = GetFieldsForType(type);
        foreach (var field in fields)
        {
            var fieldName = field.Name;

            if (!inputValues.TryGetValue(fieldName, out var value))
                continue;

            field.SetValue(output, DeserializeObject(value, field.FieldType));
        }
    }
}
