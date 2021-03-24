using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal sealed class JsonPropertyInfo<T> : IJsonMemberInfo
    {
        public JsonPropertyInfo(PropertyInfo property, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
        {
            Options = options;
            TypedConverter = (JsonConverter<T>) converter;
            IgnoreCondition = ignoreCondition;
            Get = CreateGetter(property);
            Set = CreateSetter(property);
            PropertyName = GetName(property);
            PropertyType = property.PropertyType;
            Ignored = IsIgnored(property);
        }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public JsonIgnoreCondition? IgnoreCondition { get; }

        public Func<object, T>? Get { get; }

        public Action<object, T>? Set { get; }

        public string PropertyName { get; }

        public Type PropertyType { get; }

        public bool Ignored { get; }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = TypedConverter.Read(ref reader, PropertyType, Options);

            Set(resource, value!);
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
            if (Get == null)
            {
                return;
            }

            var value = Get(resource);

            writer.WritePropertyName(PropertyName);
            TypedConverter.Write(writer, value, Options);
        }

        private string GetName(MemberInfo member)
        {
            var nameAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            if (Options.PropertyNamingPolicy != null)
            {
                return Options.PropertyNamingPolicy.ConvertName(member.Name);
            }

            return member.Name;
        }

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

        private bool IsPublic(MethodInfo? method)
        {
            return method != null && method.IsPublic;
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
