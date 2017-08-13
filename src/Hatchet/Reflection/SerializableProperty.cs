using System.Reflection;

namespace Hatchet.Reflection
{
    internal class SerializableProperty : ISerializableMember
    {
        private readonly PropertyInfo _property;
        private readonly object _obj;

        public SerializableProperty(PropertyInfo property, object obj)
        {
            _property = property;
            _obj = obj;
        }

        public string Name => _property.Name;
            
        public object Value => _property.GetValue(_obj);
            
        public bool IsValueAbstract => _property.PropertyType.IsAbstract;
    }
}