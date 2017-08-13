using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private static List<Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>> DeserializationRules { get; }
        
        private static List<Tuple<Func<object, bool>, Action<SerializationContext>>> SerializationRules { get; }
        
        static HatchetConvert()
        {
            SerializationRules = new List<Tuple<Func<object, bool>, Action<SerializationContext>>>
            {
                MakeSerialiser(o => o is string, c => c.Printer.AppendString(c.Input as string)),
                MakeSerialiser(o => o is DateTime, c => c.Printer.AppendDateTime((DateTime)c.Input)),
                MakeSerialiser(o => o.GetType().IsArray, c => SerializeArray(c.Input, c.Printer)),
                MakeSerialiser(o => o is IDictionary, c => SerializeDictionary(c.Printer, (IDictionary)c.Input)),
                MakeSerialiser(o => o.GetType().GenericTypeArguments.Length == 1, SerializeGenericEnumerable),
                MakeSerialiser(o => typeof (ICollection).IsAssignableFrom(o.GetType()), SerializeCollection),
                MakeSerialiser(o => IsSimpleValue(o.GetType()), c => c.Printer.Append(c.Input)),
                MakeSerialiser(o => o.GetType().IsEnum, c => c.Printer.AppendEnum(c.Input)),
                MakeSerialiser(o => o.GetType().IsClass || o.GetType().IsValueType, SerializeClassOrStruct)
            };
            
            DeserializationRules = new List<Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>>
            {
                MakeDeserializer(c => c.OutputType == typeof(string), c => c.Input),
                MakeDeserializer(c => c.OutputType == typeof(object), c => c.Input),
                MakeDeserializer(c => c.OutputType.IsArray, DeserializeArray),
                MakeDeserializer(c => typeof(IDictionary).IsAssignableFrom(c.OutputType), DeserializeDictionary),
                MakeDeserializer(c => IsGenericCollection(c.Input, c.OutputType), DeserializeGenericCollection),
                MakeDeserializer(c => c.OutputType.IsEnum, DeserializeEnum),
                MakeDeserializer(c => IsSimpleValueType(c.OutputType), DeserializeSimpleValue),
                MakeDeserializer(c => IsNullableValueType(c.OutputType), DeserializeNullableValueType),
                MakeDeserializer(c => c.OutputType == typeof(Guid), DeserializeGuid),
                MakeDeserializer(c => IsComplexType(c.OutputType), DeserializeComplexType)
            };
        }

        private static Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>> MakeDeserializer(
            Func<DeserializationContext, bool> test, Func<DeserializationContext, object> action)
        {
            return new Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>( test, action);
        }
        
        private static Tuple<Func<object, bool>, 
            Action<SerializationContext>> MakeSerialiser(Func<object, bool> test, 
            Action<SerializationContext> action)
        {
            return new Tuple<Func<object, bool>, Action<SerializationContext>>(test, action);
        }
    }
}