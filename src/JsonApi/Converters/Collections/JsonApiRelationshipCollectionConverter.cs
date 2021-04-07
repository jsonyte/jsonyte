using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiRelationshipCollectionConverter<T, TElement> : JsonApiRelationshipDetailsConverter<T>
    {
        public Type? ElementType { get; } = typeof(TElement);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override T? Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationships = default(T);

            var relationshipState = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref relationshipState);

                if (name == JsonApiMembers.Data)
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

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray(JsonApiArrayCode.Relationships);

            var relationships = new List<TElement>();

            var converter = options.GetRelationshipConverter<TElement>();

            while (reader.IsInArray())
            {
                var resource = converter.ReadWrapped(ref reader, ref tracked, ElementType!, default, options);

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
