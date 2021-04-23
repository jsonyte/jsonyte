using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters;
using Jsonyte.Serialization.Contracts;

namespace Jsonyte.Serialization.Metadata
{
    internal abstract class JsonMemberInfo
    {
        public abstract string Name { get; }

        public abstract string MemberName { get; }

        public abstract Type MemberType { get; }

        public abstract JsonEncodedText NameEncoded { get; }

        public abstract JsonConverter Converter { get; }

        public abstract bool Ignored { get; }

        public abstract bool IsRelationship { get; }

        public abstract object? GetValue(object resource);

        public abstract void SetValue(object resource, object? value);

        public abstract void Read(ref Utf8JsonReader reader, object resource);

        public abstract void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource);

        public abstract bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default);

        public abstract void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection);

        public abstract void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource);
    }

    [DebuggerDisplay(@"\{{Name,nq}\}")]
    internal abstract class JsonMemberInfo<T> : JsonMemberInfo
    {
        private readonly ConcurrentDictionary<Type, RelationshipType> relationshipTypes = new();

        private JsonApiRelationshipDetailsConverter<T>? relationshipConverter;

        private bool isRelationship;

        protected JsonMemberInfo(MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
        {
            var name = GetName(member, options);

            Options = options;
            MemberType = memberType;
            Converter = converter;
            MemberName = member.Name;
            Name = name;
            NameEncoded = JsonEncodedText.Encode(name);
            IsPrimitiveType = GetIsPrimitiveType(memberType);
            IsNumericType = GetIsNumericType(memberType);
            TypedConverter = (JsonConverter<T>) converter;
            WrappedConverter = converter as WrappedJsonConverter<T>;
            IgnoreCondition = ignoreCondition;
            isRelationship = GetIsRelationship(memberType);
        }

        public JsonSerializerOptions Options { get; }

        public override string Name { get; }

        public override JsonEncodedText NameEncoded { get; }

        public override string MemberName { get; }

        public override Type MemberType { get; }

        public override JsonConverter Converter { get; }

        public bool IsPrimitiveType { get; }

        public bool IsNumericType { get; }

        public JsonConverter<T> TypedConverter { get; }

        public WrappedJsonConverter<T>? WrappedConverter { get; }

        public override bool IsRelationship => isRelationship;

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

        public override object? GetValue(object resource)
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

        public override void Read(ref Utf8JsonReader reader, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = Options.NumberHandling != JsonNumberHandling.Strict && IsNumericType
                ? JsonSerializer.Deserialize<T>(ref reader, Options)
                : TypedConverter.Read(ref reader, MemberType, Options);

            if (Options.IgnoreNullValues && value == null)
            {
                return;
            }

            Set(resource, value!);
        }

        public override void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
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

        public override bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default)
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
                isRelationship = true;
            }

            if (isRelationship)
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

        public override void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection)
        {
            if (Get == null || Ignored || !isRelationship)
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

        public override void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
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

        public override void SetValue(object resource, object? value)
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

        private string GetName(MemberInfo member, JsonSerializerOptions options)
        {
            var nameAttribute = member.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            if (options.PropertyNamingPolicy != null)
            {
                return options.PropertyNamingPolicy.ConvertName(member.Name);
            }

            return member.Name;
        }

        private bool GetIsPrimitiveType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive ||
                   underlyingType == typeof(string) ||
                   underlyingType == typeof(decimal) ||
                   underlyingType == typeof(DateTime) ||
                   underlyingType == typeof(Guid) ||
                   underlyingType == typeof(TimeSpan);
        }

        private bool GetIsNumericType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType == typeof(decimal) ||
                   underlyingType == typeof(float) ||
                   underlyingType == typeof(double) ||
                   underlyingType == typeof(byte) ||
                   underlyingType == typeof(short) ||
                   underlyingType == typeof(ushort) ||
                   underlyingType == typeof(int) ||
                   underlyingType == typeof(uint) ||
                   underlyingType == typeof(long) ||
                   underlyingType == typeof(ulong);
        }

        protected bool IsPublic(MethodInfo? method)
        {
            return method != null && method.IsPublic;
        }

        private bool GetIsRelationship(Type type)
        {
            return type.IsResourceIdentifier() || type.IsResourceIdentifierCollection() || type.IsExplicitRelationship();
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
