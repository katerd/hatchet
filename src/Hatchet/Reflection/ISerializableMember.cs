namespace Hatchet.Reflection
{
    internal interface ISerializableMember
    {
        string Name { get; }
        object Value { get; }
        bool IsValueAbstract { get; }
    }
}