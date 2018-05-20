using System.Reflection;

namespace Hatchet.Reflection
{
    internal class SerializableField : ISerializableMember
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _obj;

        public SerializableField(FieldInfo fieldInfo, object obj)
        {
            _obj = obj;
            _fieldInfo = fieldInfo;
        }

        public string Name => _fieldInfo.Name;
            
        public object Value => _fieldInfo.GetValue(_obj);

        public bool IsValueAbstract => _fieldInfo.FieldType.IsAbstract;
    }
}