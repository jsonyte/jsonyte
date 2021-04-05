using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiRelationshipCollectionConverter<T, TElement> : JsonApiRelationshipConverterBase<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
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
                    relationships = ReadData(ref reader, ref state, options);
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
            throw new NotImplementedException();
        }

        private T? ReadData(ref Utf8JsonReader reader, ref JsonApiState state, JsonSerializerOptions options)
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
                var resource = converter.ReadWrapped(ref reader, ref state, typeof(TElement), default, options);

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
            var category = typeof(T).GetTypeCategory();

            if (category == JsonTypeCategory.Array)
            {
                return relationships.ToArray();
            }

            return relationships;
        }
    }
}
