using System;
using System.Collections;
using System.Collections.Generic;

namespace Hatchet;

public static partial class HatchetConvert
{
    private static readonly List<Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>>
        DeserializationRules;
        
    static HatchetConvert()
    {
        DeserializationRules =
        [
            MakeDeserializer(c => c.OutputType == typeof(string), c => c.Input),
            MakeDeserializer(c => c.OutputType == typeof(object), c => c.Input),
            MakeDeserializer(c => c.OutputType.IsArray, DeserializeArray),
            MakeDeserializer(
                c => typeof(IDictionary).IsAssignableFrom(c.OutputType),
                DeserializeDictionary),
            MakeDeserializer(
                c => IsGenericCollection(c.Input, c.OutputType),
                DeserializeGenericCollection),
            MakeDeserializer(c => c.OutputType.IsEnum, DeserializeEnum),
            MakeDeserializer(c => IsSimpleValueType(c.OutputType), DeserializeSimpleValue),
            MakeDeserializer(c => IsNullableValueType(c.OutputType), DeserializeNullableValueType),
            MakeDeserializer(c => c.OutputType == typeof(Guid), DeserializeGuid),
            MakeDeserializer(c => IsComplexType(c.OutputType), DeserializeComplexType)
        ];
    }

    private static Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>> MakeDeserializer(
        Func<DeserializationContext, bool> test, Func<DeserializationContext, object> action)
    {
        return new Tuple<Func<DeserializationContext, bool>, Func<DeserializationContext, object>>( test, action);
    }
}
