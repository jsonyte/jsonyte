using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters;
using Jsonyte.Converters.Objects;

namespace Jsonyte.Serialization
{
    [DebuggerDisplay(@"\{{Name,nq}\}")]
    internal abstract class JsonMemberInfo<T> : IJsonMemberInfo
    {
        private JsonApiRelationshipDetailsConverter<T>? relationshipConverter;

        private IAnonymousRelationshipConverter? anonymousRelationshipConverter;

        private bool firstRead;

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
            IsRelationship = GetIsRelationship(memberType);
            IsPossiblyAnonymous = MemberType == typeof(object);
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

        public Type? ValueType { get; private set; }

        public abstract bool Ignored { get; }

        public JsonConverter Converter { get; }

        public bool IsRelationship { get; private set; }

        public bool IsPossiblyAnonymous { get; }

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

        public bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default)
        {
            if (Get == null || Ignored)
            {
                return false;
            }

            var value = Get(resource);

            if (Options.IgnoreNullValues && value == null)
            {
                return false;
            }

            CheckAttributeIsResource(value);

            if (IsRelationship)
            {
                return false;
            }

            if (!section.EncodedUtf8Bytes.IsEmpty)
            {
                writer.WritePropertyName(section);
                writer.WriteStartObject();
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

            return true;
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

                if (anonymousRelationshipConverter != null)
                {
                    anonymousRelationshipConverter.Write(writer, ref tracked, value);
                }
                else
                {
                    RelationshipConverter.Write(writer, ref tracked, new RelationshipResource<T>(value), Options);
                }
            }
        }

        public void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
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
                RelationshipConverter.WriteWrapped(writer, ref tracked, new RelationshipResource<T>(value), Options);
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

        private bool GetIsRelationship(Type type)
        {
            return type.IsResourceIdentifier() || type.IsResourceIdentifierCollection() || type.IsNaturalRelationship();
        }

        private IAnonymousRelationshipConverter GetAnonymousRelationshipConverter(Type type)
        {
            var converterType = typeof(JsonApiAnonymousRelationshipConverter<>).MakeGenericType(type);

            return (IAnonymousRelationshipConverter) Activator.CreateInstance(converterType, Options);
        }

        private void CheckAttributeIsResource(T value)
        {
            if (firstRead || value == null)
            {
                return;
            }

            firstRead = true;

            var actualType = value.GetType();

            // Anonymous object types sometimes aren't declared until we inspect the actual object
            if (IsPossiblyAnonymous && actualType != MemberType && GetIsRelationship(actualType))
            {
                ValueType = actualType;
                IsRelationship = true;

                anonymousRelationshipConverter = GetAnonymousRelationshipConverter(actualType);
            }
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
