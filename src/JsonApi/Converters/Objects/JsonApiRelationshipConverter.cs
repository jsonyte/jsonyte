using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiRelationshipConverter : JsonConverter<JsonApiRelationship>
    {
        public override JsonApiRelationship Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationship = new JsonApiRelationship();

            var state = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Links)
                {
                    relationship.Links = reader.Read<JsonApiRelationshipLinks>(options);
                }
                else if (name == JsonApiMembers.Data)
                {
                    relationship.Data = ReadData(ref reader, options);
                }
                else if (name == JsonApiMembers.Meta)
                {
                    relationship.Meta = reader.Read<JsonApiMeta>(options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return relationship;
        }

        private JsonApiResourceIdentifier[]? ReadData(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.IsObject())
            {
                var identifier = reader.Read<JsonApiResourceIdentifier>(options);

                return new[] {identifier};
            }

            var identifiers = new List<JsonApiResourceIdentifier>();

            reader.ReadArray(JsonApiArrayCode.ResourceIdentifiers);

            while (reader.IsArray())
            {
                var identifier = reader.Read<JsonApiResourceIdentifier>(options);

                identifiers.Add(identifier);

                reader.Read();
            }

            return identifiers.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiRelationship value, JsonSerializerOptions options)
        {
            ValidateRelationship(value);

            writer.WriteStartObject();

            if (value.Data != null)
            {
                writer.WritePropertyName(JsonApiMembers.Data);

                if (value.Data.Length == 1)
                {
                    JsonSerializer.Serialize(writer, value.Data[0], options);
                }
                else
                {
                    writer.WriteStartArray();

                    foreach (var identifier in value.Data)
                    {
                        JsonSerializer.Serialize(writer, identifier, options);
                    }

                    writer.WriteEndArray();
                }
            }

            if (value.Links != null)
            {
                writer.WritePropertyName(JsonApiMembers.Links);
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName(JsonApiMembers.Meta);
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            writer.WriteEndObject();
        }

        [AssertionMethod]
        private void ValidateRelationship(JsonApiRelationship relationship)
        {
            if (relationship.Data == null && relationship.Links == null && relationship.Meta == null)
            {
                throw new JsonApiFormatException("JSON:API relationship must contain a 'links', 'data' or 'meta' member");
            }
        }
    }
}
