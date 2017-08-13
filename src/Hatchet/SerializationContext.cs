namespace Hatchet
{
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