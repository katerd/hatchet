using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private static List<Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>> DeserializationRules { get; }

        internal static List<Tuple<Func<object, bool>, Action<SerializationContext>>> SerializationRules { get; }
        
        static HatchetConvert()
        {
            SerializationRules = new List<Tuple<Func<object, bool>, Action<SerializationContext>>>
            {
                MakeSerialiser(o => o.GetType().IsArray, SerializeArray),
                MakeSerialiser(o => o is IDictionary, SerializeDictionary),
                MakeSerialiser(o => o.GetType().GenericTypeArguments.Length == 1, SerializeGenericEnumerable),
                MakeSerialiser(o => o is ICollection, SerializeCollection),
                MakeSerialiser(o => o is bool, SerializeBoolean),
                MakeSerialiser(o => IsSimpleValue(o.GetType()), SerializeSimpleValue),
                MakeSerialiser(o => o.GetType().IsEnum, SerializeEnum),
                MakeSerialiser(ShouldSerializeClassOrStruct, SerializeClassOrStruct)
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

        private static bool ShouldSerializeClassOrStruct(object value)
        {
            return value.GetType().IsClass || value.GetType().IsValueType;
        }

        internal static void SerializeString(string input, SerializationContext c)
        {
            c.Printer.AppendString(input);
        }

        internal static void SerializeDateTime(DateTime input, SerializationContext c)
        {
            c.Printer.AppendDateTime(input);
        }

        private static void SerializeSimpleValue(SerializationContext c)
        {
            c.Printer.Append(c.Input);
        }

        private static void SerializeEnum(SerializationContext c)
        {
            c.Printer.AppendEnum(c.Input);
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