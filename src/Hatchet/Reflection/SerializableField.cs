using System.Reflection;

namespace Hatchet.Reflection;

internal class SerializableField(FieldInfo fieldInfo, object obj) : ISerializableMember
{
    public string Name => fieldInfo.Name;
    public object Value => fieldInfo.GetValue(obj);
    public bool IsValueAbstract => fieldInfo.FieldType.IsAbstract;
}
