using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using JsonApi.Serialization;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiRelationshipConverter<T> : JsonApiRelationshipDetailsConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override T? Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationship = default(T);

            var relationshipState = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref relationshipState);

                if (name == JsonApiMembers.Data)
                {
                    relationship = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            relationshipState.Validate();

            return relationship;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            var identifier = reader.Read<JsonApiResourceIdentifier>(options);

            if (tracked.TryGetIncluded(identifier, out var value))
            {
                return (T) value.Value;
            }

            var info = options.GetClassInfo(typeToConvert);
            var relationship = info.Creator();

            if (relationship == null)
            {
                return default;
            }

            info.GetMember(JsonApiMembers.Id).Write(relationship, identifier.Id);
            info.GetMember(JsonApiMembers.Type).Write(relationship, identifier.Type);

            var converter = options.GetValueConverter<T>();

            tracked.AddIncluded(identifier, converter, relationship);

            return (T) relationship;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

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

            reader.ReadArray("resource identifiers");

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
