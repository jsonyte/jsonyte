using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters;
using Jsonyte.Serialization.Contracts;
#pragma warning disable SYSLIB0020

namespace Jsonyte.Serialization.Metadata
{
    internal abstract class JsonMemberInfo
    {
        public abstract string Name { get; }

        public abstract Type MemberType { get; }

        public abstract JsonEncodedText NameEncoded { get; }

        public abstract bool Ignored { get; }

        public abstract bool IsRelationship { get; }

        public abstract object? GetValue(object resource);

        public abstract void SetValue(object resource, object? value);

        public abstract void Read(ref Utf8JsonReader reader, object resource);

        public abstract void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource);

        public abstract void ReadRelationshipWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource);

        public abstract bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default);

        public abstract void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection);

        public abstract void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource);
    }

    [DebuggerDisplay(@"\{{Name,nq}\}")]
    internal abstract class JsonMemberInfo<T> : JsonMemberInfo
    {
        private readonly ConcurrentDictionary<Type, RelationshipType> relationshipTypes = new();

        private JsonApiRelationshipDetailsConverter<T>? relationshipConverter;

        private JsonConverter<InlineResource<T>>? inlineResourceConverter;

        private bool isRelationship;

        protected JsonMemberInfo(MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonConverter converter, JsonSerializerOptions options)
        {
            var name = GetName(member, options);

            Options = options;
            MemberType = memberType;
            Name = name;
            NameEncoded = JsonEncodedText.Encode(name);
            IsPrimitiveType = memberType.GetIsPrimitive();
            IsNumericType = GetIsNumericType(memberType);
            CanBeNull = memberType.CanBeNull();
            TypedConverter = (JsonConverter<T>) converter;
            WrappedConverter = converter as WrappedJsonConverter<T>;
            IgnoreDefaultValuesOnRead = GetIgnoreValuesOnRead(ignoreCondition, CanBeNull);
            IgnoreDefaultValuesOnWrite = GetIgnoreValuesOnWrite(ignoreCondition, CanBeNull);
            isRelationship = memberType.IsRelationship();
        }

        public JsonSerializerOptions Options { get; }

        public override string Name { get; }

        public override JsonEncodedText NameEncoded { get; }

        public override Type MemberType { get; }

        public bool IsPrimitiveType { get; }

        public bool IsNumericType { get; }

        public bool CanBeNull { get; }

        public bool IgnoreDefaultValuesOnRead { get; }

        public bool IgnoreDefaultValuesOnWrite { get; }

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

        public JsonConverter<InlineResource<T>> InlineResourceConverter
        {
            get
            {
                return inlineResourceConverter ??= Options.GetConverter<InlineResource<T>>();
            }
        }

        public abstract Func<object, T>? Get { get; }

        public abstract Action<object, T>? Set { get; }

        public override object? GetValue(object resource)
        {
            if (Get == null || Ignored)
            {
                return null;
            }

            var value = Get(resource);

            if (IgnoreDefaultValuesOnWrite && value == null)
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

            T? value;

            if (IsInlineResource())
            {
                value = InlineResourceConverter.Read(ref reader, MemberType, Options).Resource;
            }
            else
            {
                value = Options.NumberHandling > 0 && (IsNumericType || MemberType == JsonApiTypes.Object)
                    ? JsonSerializer.Deserialize<T>(ref reader, Options)
                    : TypedConverter.Read(ref reader, MemberType, Options);
            }

            if (IgnoreDefaultValuesOnRead && value == null)
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

            var value = RelationshipConverter.Read(ref reader, ref tracked, Options);

            if (IgnoreDefaultValuesOnRead && value.Resource == null)
            {
                return;
            }

            Set(resource, value.Resource!);
        }

        public override void ReadRelationshipWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            if (Set == null)
            {
                return;
            }

            var value = RelationshipConverter.ReadWrapped(ref reader, ref tracked, default, Options);

            if (IgnoreDefaultValuesOnRead && value.Resource == null)
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

            if (IgnoreDefaultValuesOnWrite && value == null)
            {
                return false;
            }

            if (IgnoreDefaultValuesOnWrite && value != null && EqualityComparer<T>.Default.Equals(default!, value))
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
            else if (Options.NumberHandling > 0 && (IsNumericType || GetIsNumericType(value?.GetType())))
            {
                JsonSerializer.Serialize(writer, value, Options);
            }
            else if (MemberType == JsonApiTypes.Object)
            {
                // The value is probably an anonymous object, but isn't a relationship, so it's
                // safer to let System.Text.Json handle it
                JsonSerializer.Serialize(writer, value, Options);
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

            if (IgnoreDefaultValuesOnWrite && value == null)
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

                if (relationshipType == RelationshipType.Declared)
                {
                    RelationshipConverter.Write(writer, ref tracked, new RelationshipResource<T>(value), Options);
                }
                else
                {
                    Options.GetAnonymousRelationshipConverter(value.GetType()).Write(writer, ref tracked, value, Options);
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

            if (IgnoreDefaultValuesOnWrite && value == null)
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
                    Options.GetAnonymousRelationshipConverter(value.GetType()).WriteWrapped(writer, ref tracked, value, Options);
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

            if (IgnoreDefaultValuesOnRead && value == null)
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

        private bool GetIsNumericType(Type? type)
        {
            if (type == null)
            {
                return false;
            }

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType == typeof(decimal) ||
                   underlyingType == typeof(float) ||
                   underlyingType == typeof(double) ||
                   underlyingType == typeof(byte) ||
                   underlyingType == typeof(sbyte) ||
                   underlyingType == typeof(short) ||
                   underlyingType == typeof(ushort) ||
                   underlyingType == typeof(int) ||
                   underlyingType == typeof(uint) ||
                   underlyingType == typeof(long) ||
                   underlyingType == typeof(ulong);
        }

        private bool GetIgnoreValuesOnRead(JsonIgnoreCondition? ignoreCondition, bool canBeNull)
        {
            return Options.IgnoreNullValues && canBeNull;
        }

        private bool GetIgnoreValuesOnWrite(JsonIgnoreCondition? ignoreCondition, bool canBeNull)
        {
            if (ignoreCondition != null)
            {
                return ignoreCondition switch
                {
                    JsonIgnoreCondition.WhenWritingDefault => true,
                    JsonIgnoreCondition.WhenWritingNull when canBeNull => true,
                    _ => false
                };
            }

            if (Options.IgnoreNullValues)
            {
                return canBeNull;
            }

            return Options.DefaultIgnoreCondition switch
            {
                JsonIgnoreCondition.WhenWritingNull => canBeNull,
                JsonIgnoreCondition.WhenWritingDefault => true,
                _ => false
            };
        }

        protected bool IsPublic(MethodInfo? method)
        {
            return method != null && method.IsPublic;
        }

        private RelationshipType GetRelationshipType(T value)
        {
            if (value == null || IsPrimitiveType)
            {
                return RelationshipType.None;
            }

            // Anonymous object types or implementations of an interface aren't declared until we inspect the actual value

            // Assuming the type is not null, there are 4 possibilities here:
            // 1. The type is declared as a relationship, in which case IsRelationship should be true already
            // 2. The type is declared as object or interface but the value is a relationship or explicit relationship
            // 3. The type is declared as collection of a type that is a relationship
            // 4. The type is declared as collection of IEnumerable with an unknown type and we need to enumerate to see if contains relationships
            var valueType = value.GetType();

            return relationshipTypes.GetOrAdd(valueType, x =>
            {
                // 1. The value type is the same as the declared type
                if (x == MemberType)
                {
                    return RelationshipType.Declared;
                }

                // Dictionaries are always treated as dictionaries, they can never be relationships
                if (typeof(IDictionary).IsAssignableFrom(x))
                {
                    return RelationshipType.None;
                }

                // 2. The value is a relationship or explicit relationship
                if (x.IsResourceIdentifier() || x.IsExplicitRelationshipByMembers())
                {
                    return RelationshipType.Object;
                }

                var collectionType = x.GetCollectionElementType();

                if (collectionType == null)
                {
                    return RelationshipType.None;
                }

                // 3. The type is a collection of a declared type that is a relationship
                if (collectionType.IsResourceIdentifier())
                {
                    return RelationshipType.TypedCollection;
                }

                // 4. The type of the collection is Object (which usually means an anonymous type) and so we need to
                // enumerate to find out what the real collection type is
                if (collectionType == JsonApiTypes.Object)
                {
                    return RelationshipType.PotentialCollection;
                }

                return RelationshipType.None;
            });
        }

        private bool IsInlineResource()
        {
            return TypedConverter is WrappedResourceJsonConverter<T>;
        }
    }
}
