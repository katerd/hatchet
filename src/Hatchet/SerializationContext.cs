namespace Hatchet
{
    internal struct SerializationContext
    {
        public object Input { get; }
        public Serializer Printer { get; }
        public bool ForceClassName { get; }
        public SerializationContext(object input, Serializer printer, bool forceClassName)
        {
            Input = input;
            Printer = printer;
            ForceClassName = forceClassName;
        }
    }
}