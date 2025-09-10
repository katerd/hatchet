namespace Hatchet;

internal readonly struct SerializationContext(bool forceClassName)
{
    public bool ForceClassName { get; } = forceClassName;
}
