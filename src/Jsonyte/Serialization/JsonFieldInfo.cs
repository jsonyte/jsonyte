using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization
{
    internal class JsonFieldInfo<T> : JsonMemberInfo<T>
    {
        public JsonFieldInfo(FieldInfo field, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
            : base(field, field.FieldType, ignoreCondition, converter, options)
        {
            Get = CreateGetter(field);
            Set = CreateSetter(field);
            Ignored = IsIgnored(field);
        }

        public override Func<object, T>? Get { get; }

        public override Action<object, T>? Set { get; }

        public override bool Ignored { get; }

        private Func<object, T> CreateGetter(FieldInfo field)
        {
            return Options.GetMemberAccessor().CreateFieldGetter<T>(field);
        }

        private Action<object, T>? CreateSetter(FieldInfo field)
        {
            if (IsReadOnly(field))
            {
                return null;
            }

            return Options.GetMemberAccessor().CreateFieldSetter<T>(field);
        }

        private bool IsIgnored(FieldInfo field)
        {
            return IsReadOnly(field) && Options.IgnoreReadOnlyFields;
        }

        private bool IsReadOnly(FieldInfo field)
        {
            return field.IsInitOnly;
        }
    }
}
