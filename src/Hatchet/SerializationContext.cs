namespace Hatchet
{
    internal struct SerializationContext
    {
        public bool ForceClassName { get; }
        
        public SerializationContext(bool forceClassName)
        {
            ForceClassName = forceClassName;
        }
    }
}