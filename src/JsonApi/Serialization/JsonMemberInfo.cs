using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi.Serialization
{
    internal abstract class JsonMemberInfo<T> : IJsonMemberInfo
    {
        private JsonApiRelationshipDetailsConverter<T>? relationshipConverter;

        protected JsonMemberInfo(MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
        {
            Options = options;
            Converter = converter;
            TypedConverter = (JsonConverter<T>) converter;
            WrappedConverter = converter as WrappedJsonConverter<T>;
            IgnoreCondition = ignoreCondition;
            Name = GetName(member);
            NameEncoded = JsonEncodedText.Encode(Name);
            MemberName = member.Name;
            MemberType = memberType;
            IsRelationship = memberType.IsResourceIdentifier() || memberType.IsResourceIdentifierCollection();
        }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public WrappedJsonConverter<T>? WrappedConverter { get; }

        public JsonApiRelationshipDetailsConverter<T> RelationshipConverter
        {
            get
            {
                return relationshipConverter ??= Options.GetRelationshipConverter<T>();
            }
        }

        public JsonIgnoreCondition? IgnoreCondition { get; }

        public abstract Func<object, T>? Get { get; }

        public abstract Action<object, T>? Set { get; }
        
        public string Name { get; }

        public JsonEncodedText NameEncoded { get; }

        public string MemberName { get; }

        public Type MemberType { get; }

        public abstract bool Ignored { get; }

        public JsonConverter Converter { get; }

        public bool IsRelationship { get; }

        public object? GetValue(object resource)
        {
            if (Get == null || Ignored)
            {
                return null;
            }

            var value = Get(resource);

            if (Options.IgnoreNullValues && value == null)
            {
                return null;
            }

            return value;
        }

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

        public void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = RelationshipConverter.Read(ref reader, ref tracked, MemberType, Options);

            if (Options.IgnoreNullValues && value.Resource == null)
            {
                return;
            }

            Set(resource, value.Resource!);
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

        public void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
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

            writer.WritePropertyName(NameEncoded);

            if (WrappedConverter != null)
            {
                WrappedConverter.WriteWrapped(writer, ref tracked, value, Options);
            }
            else
            {
                TypedConverter.Write(writer, value, Options);
            }
        }

        public void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection)
        {
            if (Get == null || Ignored || !IsRelationship)
            {
                return;
            }

            var value = Get(resource);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            if (value != null)
            {
                if (!wroteSection)
                {
                    writer.WritePropertyName(JsonApiMembers.RelationshipsEncoded);
                    writer.WriteStartObject();

                    wroteSection = true;
                }

                writer.WritePropertyName(NameEncoded);
                RelationshipConverter.Write(writer, ref tracked, new RelationshipResource<T>(value), Options);
            }
        }

        public void SetValue(object resource, object? value)
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
