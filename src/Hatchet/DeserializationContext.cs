using System;

namespace Hatchet;

internal struct DeserializationContext(object input, Type outputType)
{
    public readonly object Input = input;
    public readonly Type OutputType = outputType;
}
