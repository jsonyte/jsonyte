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
            Converter = converter;
            TypedConverter = (JsonConverter<T>) converter;
            IgnoreCondition = ignoreCondition;
            Name = GetName(member);
            MemberName = member.Name;
            MemberType = memberType;
        }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public JsonIgnoreCondition? IgnoreCondition { get; }

        public abstract Func<object, T>? Get { get; }

        public abstract Action<object, T>? Set { get; }
        
        public string Name { get; }

        public string MemberName { get; }

        public Type MemberType { get; }

        public abstract bool Ignored { get; }

        public JsonConverter Converter { get; }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = TypedConverter.Read(ref reader, MemberType, Options);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            Set(resource, value!);
        }

        public void ReadRelationship(ref Utf8JsonReader reader, ref JsonApiState state, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var converter = Options.GetRelationshipConverter<T>();

            var value = converter.Read(ref reader, ref state, MemberType, Options);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            Set(resource, value!);
        }

        public void ReadExisting(ref Utf8JsonReader reader, ref JsonApiState state, object existingValue)
        {
            if (Set == null)
            {
                return;
            }

            var converter = Options.GetWrappedConverter<T>();

            converter.ReadWrapped(ref reader, ref state, MemberType, (T) existingValue, Options);
        }

        public object? Read(ref Utf8JsonReader reader)
        {
            if (Set == null)
            {
                return null;
            }

            var value = TypedConverter.Read(ref reader, MemberType, Options);

            if (Options.IgnoreNullValues && value == null)
            {
                return null;
            }

            return value;
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
            if (Get == null || Ignored)
            {
                return;
            }

            var value = Get(resource);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            writer.WritePropertyName(Name);
            TypedConverter.Write(writer, value, Options);
        }

        public void Write(object resource, object? value)
        {
            if (Set == null)
            {
                return;
            }

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            Set(resource, (T) value!);
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
