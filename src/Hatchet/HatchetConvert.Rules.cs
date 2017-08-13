using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private static List<Tuple<Func<Context, bool>, Func<Context, object>>> DeserializationRules { get; }
        
        private static List<Tuple<Func<object, bool>, Action<object, PrettyPrinter, bool>>> SerializationRules { get; }
        
        static HatchetConvert()
        {
            SerializationRules = new List<Tuple<Func<object, bool>, Action<object, PrettyPrinter, bool>>>
            {
                MakeSerialiser(o => o is string, (o, pp, b) => pp.AppendString(o as string)),
                MakeSerialiser(o => o is DateTime, (o, pp, b) => pp.AppendDateTime((DateTime)o)),
                MakeSerialiser(o => o.GetType().IsArray, (o, pp, b) => SerializeArray(o, pp)),
                MakeSerialiser(o => o is IDictionary, (o, pp, b) => SerializeDictionary(pp, (IDictionary)o)),
                MakeSerialiser(o => o.GetType().GenericTypeArguments.Length == 1, SerializeGenericEnumerable),
                MakeSerialiser(o => typeof (ICollection).IsAssignableFrom(o.GetType()), SerializeCollection),
                MakeSerialiser(o => IsSimpleValue(o.GetType()), (o, pp, b) => pp.Append(o)),
                MakeSerialiser(o => o.GetType().IsEnum, (o, pp, b) => pp.AppendEnum(o)),
                MakeSerialiser(o => o.GetType().IsClass || o.GetType().IsValueType, SerializeClassOrStruct)
            };
            
            DeserializationRules = new List<Tuple<Func<Context, bool>, Func<Context, object>>>
            {
                MakeDeserializer(c => c.OutputType == typeof(string), c => c.Input),
                MakeDeserializer(c => c.OutputType == typeof(object), c => c.Input),
                MakeDeserializer(c => c.OutputType.IsArray, c => DeserializeArray(c.Input, c.OutputType)),
                MakeDeserializer(c => typeof(IDictionary).IsAssignableFrom(c.OutputType), c => DeserializeDictionary(c.Input, c.OutputType)),
                MakeDeserializer(c => IsGenericCollection(c.Input, c.OutputType), c => DeserializeGenericCollection(c.Input, c.OutputType)),
                MakeDeserializer(c => c.OutputType.IsEnum, c => DeserializeEnum(c.Input, c.OutputType)),
                MakeDeserializer(c => IsSimpleValueType(c.OutputType), c => Convert.ChangeType(c.Input, c.OutputType)),
                MakeDeserializer(c => IsNullableValueType(c.OutputType), c=> DeserializeNullableValueType(c.Input, c.OutputType)),
                MakeDeserializer(c => c.OutputType == typeof(Guid), c => new Guid(c.Input.ToString())),
                MakeDeserializer(c => IsComplexType(c.OutputType), c => GetComplexType(c.Input, c.OutputType))
            };
        }
        
        private static Tuple<Func<Context, bool>, Func<Context, object>> MakeDeserializer(
            Func<Context, bool> test, Func<Context, object> action)
        {
            return new Tuple<Func<Context, bool>, Func<Context, object>>( test, action);
        }
    }
}