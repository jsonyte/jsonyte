using System;
using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;

namespace Jsonyte.Converters.Collections
{
    internal class JsonApiRelationshipCollectionConverter<T, TElement> : JsonApiRelationshipDetailsConverter<T>
    {
        private JsonApiRelationshipDetailsConverter<TElement>? relationshipConverter;

        public Type? ElementType { get; } = typeof(TElement);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationships = default(RelationshipResource<T>);

            var relationshipState = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref relationshipState);

                if (name.IsEqual(JsonApiMembers.DataEncoded))
                {
                    relationships = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            relationshipState.Validate();

            return relationships;
        }

        public override RelationshipResource<T> ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, RelationshipResource<T> existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray(JsonApiArrayCode.Relationships);

            var relationships = new List<TElement>();

            var converter = GetRelationshipConverter(options);

            while (reader.IsInArray())
            {
                var value = converter.ReadWrapped(ref reader, ref tracked, ElementType!, default, options);

                if (value.Resource != null)
                {
                    relationships.Add(value.Resource);
                }

                reader.Read();
            }

            return new RelationshipResource<T>((T) GetCollection(relationships));
        }

        public override void Write(Utf8JsonWriter writer, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.DataEncoded);

            WriteWrapped(writer, ref tracked, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            if (value.Resource == null)
            {
                writer.WriteNullValue();
            }
            else if (value.Resource is IEnumerable<TElement> collection)
            {
                var converter = GetRelationshipConverter(options);

                writer.WriteStartArray();

                foreach (var element in collection)
                {
                    var resource = new RelationshipResource<TElement>(element);

                    converter.WriteWrapped(writer, ref tracked, resource, options);
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonApiFormatException($"JSON:API resources collection of type '{typeof(T).Name}' must be an enumerable");
            }
        }

        private JsonApiRelationshipDetailsConverter<TElement> GetRelationshipConverter(JsonSerializerOptions options)
        {
            return relationshipConverter ??= options.GetRelationshipConverter<TElement>();
        }

        private object GetCollection(List<TElement> relationships)
        {
            return TypeCategory == JsonTypeCategory.Array
                ? relationships.ToArray()
                : relationships;
        }
    }
}
