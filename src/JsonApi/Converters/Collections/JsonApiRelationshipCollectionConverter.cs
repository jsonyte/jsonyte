using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiRelationshipCollectionConverter<T, TElement> : JsonApiRelationshipConverterBase<T>
    {
        public Type? ElementType { get; } = typeof(TElement);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var state = new JsonApiState();

            return Read(ref reader, ref state, typeToConvert, options);
        }

        public override T? Read(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationships = default(T);

            var relationshipState = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref relationshipState);

                if (name == JsonApiMembers.Data)
                {
                    relationships = ReadWrapped(ref reader, ref state, typeToConvert, default, options);
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

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray("relationships");

            var relationships = new List<TElement>();

            var converter = options.GetRelationshipConverter<TElement>();

            while (reader.IsInArray())
            {
                var resource = converter.ReadWrapped(ref reader, ref state, ElementType!, default, options);

                if (resource != null)
                {
                    relationships.Add(resource);
                }

                reader.Read();
            }

            return (T) GetCollection(relationships);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private object GetCollection(List<TElement> relationships)
        {
            return TypeCategory == JsonTypeCategory.Array
                ? relationships.ToArray()
                : relationships;
        }
    }
}
