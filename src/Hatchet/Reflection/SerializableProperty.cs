using System.Reflection;

namespace Hatchet.Reflection;

internal class SerializableProperty(PropertyInfo property, object obj) : ISerializableMember
{
    public string Name => property.Name;
    public object Value => property.GetValue(obj);
    public bool IsValueAbstract => property.PropertyType.IsAbstract;
}
