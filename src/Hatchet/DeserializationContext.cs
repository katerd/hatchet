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

    internal struct SerializationContext
    {
        public object Input { get; }
        public PrettyPrinter Printer { get; }
        public bool ForceClassName { get; }

        public SerializationContext(object input, PrettyPrinter printer, bool forceClassName)
        {
            Input = input;
            Printer = printer;
            ForceClassName = forceClassName;
        }
    }
}