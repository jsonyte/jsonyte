using System;
using System.Reflection;

namespace JsonApi.Serialization
{
    public abstract class JsonPropertyInfo
    {
        protected JsonPropertyInfo(PropertyInfo property)
        {
            Name = property.Name;
            PublicName = property.Name.ToCamelCase();
            PropertyType = property.PropertyType;
        }

        public string Name { get; }

        public string PublicName { get; }

        public Type PropertyType { get; }

        public abstract bool HasGetter { get; }

        public abstract bool HasSetter { get; }

        public abstract object GetValueAsObject(object resource);

        public abstract void SetValueAsObject(object resource, object value);
    }
}