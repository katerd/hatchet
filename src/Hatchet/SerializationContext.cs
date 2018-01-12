namespace Hatchet
{
    internal struct SerializationContext
    {
        public object Input { get; }
        public Serializer Serializer { get; }
        public bool ForceClassName { get; }
        public SerializationContext(object input, Serializer serializer, bool forceClassName)
        {
            Input = input;
            Serializer = serializer;
            ForceClassName = forceClassName;
        }
    }
}