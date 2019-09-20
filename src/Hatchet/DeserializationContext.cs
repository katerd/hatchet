using System;

namespace Hatchet
{
    internal struct DeserializationContext
    {
        public readonly object Input;
        public readonly Type OutputType;

        public DeserializationContext(object input, Type outputType)
        {
            Input = input;
            OutputType = outputType;
        }
    }
}