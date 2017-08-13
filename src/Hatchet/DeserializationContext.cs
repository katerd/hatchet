using System;

namespace Hatchet
{
    internal struct DeserializationContext
    {
        public object Input { get; }
        public Type OutputType { get; }

        public DeserializationContext(object input, Type outputType)
        {
            Input = input;
            OutputType = outputType;
        }
    }
}