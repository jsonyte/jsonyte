using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization
{
    internal sealed class JsonPropertyInfo<T> : JsonMemberInfo<T>
    {
        public JsonPropertyInfo(PropertyInfo property, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
            : base(property, property.PropertyType, ignoreCondition, converter, options)
        {
            Get = CreateGetter(property);
            Set = CreateSetter(property);
            Ignored = IsIgnored(property);
        }

        public override Func<object, T>? Get { get; }

        public override Action<object, T>? Set { get; }

        public override bool Ignored { get; }

        private Func<object, T>? CreateGetter(PropertyInfo property)
        {
            if (!IsPublic(property.GetMethod))
            {
                return null;
            }

            return Options.GetMemberAccessor().CreatePropertyGetter<T>(property);
        }

        private Action<object, T>? CreateSetter(PropertyInfo property)
        {
            if (!IsPublic(property.SetMethod))
            {
                return null;
            }

            return Options.GetMemberAccessor().CreatePropertySetter<T>(property);
        }
        
        private bool IsIgnored(PropertyInfo property)
        {
            return IsReadOnly(property) && Options.IgnoreReadOnlyProperties;
        }

        private bool IsReadOnly(PropertyInfo property)
        {
            return IsPublic(property.GetMethod) && !IsPublic(property.SetMethod);
        }
    }
}
