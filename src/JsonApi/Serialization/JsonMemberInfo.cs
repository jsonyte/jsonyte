using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal abstract class JsonMemberInfo<T> : IJsonMemberInfo
    {
        protected JsonMemberInfo(MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
        {
            Options = options;
            TypedConverter = (JsonConverter<T>) converter;
            IgnoreCondition = ignoreCondition;
            MemberName = GetName(member);
            MemberType = memberType;
        }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public JsonIgnoreCondition? IgnoreCondition { get; }

        public abstract Func<object, T>? Get { get; }

        public abstract Action<object, T>? Set { get; }

        public string MemberName { get; }

        public Type MemberType { get; }

        public abstract bool Ignored { get; }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = TypedConverter.Read(ref reader, MemberType, Options);

            Set(resource, value!);
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
            if (Get == null)
            {
                return;
            }

            var value = Get(resource);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            writer.WritePropertyName(MemberName);
            TypedConverter.Write(writer, value, Options);
        }

        protected bool IsPublic(MethodInfo? method)
        {
            return method != null && method.IsPublic;
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
    }
}
