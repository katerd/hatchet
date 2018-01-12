namespace Hatchet
{
    internal struct SerializationContext
    {
        public Serializer Serializer { get; }
        public bool ForceClassName { get; }
        
        public SerializationContext(Serializer serializer, bool forceClassName)
        {
            Serializer = serializer;
            ForceClassName = forceClassName;
        }
    }
}