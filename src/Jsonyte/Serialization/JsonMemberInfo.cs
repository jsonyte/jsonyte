using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters;

namespace Jsonyte.Serialization
{
    [DebuggerDisplay(@"\{{Name,nq}\}")]
    internal abstract class JsonMemberInfo<T> : IJsonMemberInfo
    {
        private readonly ConcurrentDictionary<Type, RelationshipType> relationshipTypes = new();

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
            NameAsBytes = Name.ToByteArray();
            MemberName = member.Name;
            MemberType = memberType;
            IsPrimitiveType = GetIsPrimitiveType(memberType);
            IsRelationship = GetIsRelationship(memberType);
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

        public byte[] NameAsBytes { get; }

        public string MemberName { get; }

        public Type MemberType { get; }

        public bool IsPrimitiveType { get; }

        public abstract bool Ignored { get; }

        public JsonConverter Converter { get; }

        public bool IsRelationship { get; private set; }

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

            var relationshipType = GetRelationshipType(value);

            if (relationshipType is RelationshipType.Object or RelationshipType.TypedCollection)
            {
                IsRelationship = true;
            }

            if (IsRelationship)
            {
                return false;
            }

            if (relationshipType == RelationshipType.PotentialCollection)
            {
                var collection = new PotentialRelationshipCollection(NameEncoded, value, false);

                Options.GetWrappedConverter<PotentialRelationshipCollection>().WriteWrapped(writer, ref tracked, collection, Options);

                return tracked.Relationships.LastWritten;
            }

            if (!section.EncodedUtf8Bytes.IsEmpty)
            {
                writer.WritePropertyName(section);
                writer.WriteStartObject();
            }

            writer.WritePropertyName(NameAsBytes);

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

                writer.WritePropertyName(NameAsBytes);

                var relationshipType = GetRelationshipType(value);

                if (relationshipType == RelationshipType.None)
                {
                    RelationshipConverter.Write(writer, ref tracked, new RelationshipResource<T>(value), Options);
                }
                else
                {
                    Options.GetAnonymousRelationshipConverter(value.GetType()).Write(writer, ref tracked, value);
                }
            }
        }

        public void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection)
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

            if (value != null)
            {
                var relationshipType = GetRelationshipType(value);

                if (relationshipType == RelationshipType.PotentialCollection)
                {
                    var collection = new PotentialRelationshipCollection(NameEncoded, value, true);

                    Options.GetWrappedConverter<PotentialRelationshipCollection>().WriteWrapped(writer, ref tracked, collection, Options);
                }
                else if (relationshipType is RelationshipType.Object or RelationshipType.TypedCollection)
                {
                    Options.GetAnonymousRelationshipConverter(value.GetType()).WriteWrapped(writer, ref tracked, value);
                }
                else
                {
                    RelationshipConverter.WriteWrapped(writer, ref tracked, new RelationshipResource<T>(value), Options);
                }
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

        private bool GetIsRelationship(Type type)
        {
            return type.IsResourceIdentifier() || type.IsResourceIdentifierCollection() || type.IsExplicitRelationship();
        }

        private bool GetIsPrimitiveType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive ||
                   underlyingType == typeof(string) ||
                   underlyingType == typeof(DateTime) ||
                   underlyingType == typeof(Guid) ||
                   underlyingType == typeof(TimeSpan);
        }

        private RelationshipType GetRelationshipType(T value)
        {
            if (value == null || IsPrimitiveType)
            {
                return RelationshipType.None;
            }

            // Anonymous object types sometimes aren't declared until we inspect the actual value

            // There are 4 possibilities here:
            // 1. The type is nullable and the value is null, this will just get skipped
            // 2. The type is declared as object but the value is a relationship or explicit relationship
            // 3. The type is declared as collection of a type that is a relationship
            // 4. The type is declared as collection of IEnumerable and we need to enumerate to see if contains relationships
            var valueType = value.GetType();

            return relationshipTypes.GetOrAdd(valueType, x =>
            {
                if (x == MemberType)
                {
                    return RelationshipType.None;
                }

                if (x.IsResourceIdentifier() || x.IsExplicitRelationship())
                {
                    return RelationshipType.Object;
                }

                if (!x.IsCollection())
                {
                    return RelationshipType.None;
                }

                var collectionType = x.GetCollectionElementType();

                if (collectionType != null && collectionType.IsResourceIdentifier())
                {
                    return RelationshipType.TypedCollection;
                }

                return RelationshipType.PotentialCollection;
            });
        }
    }
}
