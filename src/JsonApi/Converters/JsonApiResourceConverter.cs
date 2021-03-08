using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiResourceDocument<T>>(ref reader, options);

                if (document == null)
                {
                    throw new JsonApiException("Invalid JSON:API document");
                }

                return document.Data;
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var info = options.GetClassInfo(typeof(T));
            var resource = info.Creator();

            ReadResource(ref reader, info, resource, false);

            return (T) resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (writer.CurrentDepth == 0)
            {
                var document = new JsonApiDocument<T>
                {
                    Data = value
                };

                JsonSerializer.Serialize(writer, document, options);

                return;
            }

            writer.WriteStartObject();

            var info = options.GetClassInfo(typeof(T));

            var valueKeys = info.Properties.Keys
                .Except(new[] {JsonApiMembers.Id, JsonApiMembers.Type})
                .ToArray();

            if (info.Properties.TryGetValue("id", out var id))
            {
                id.Write(writer, value);
            }

            if (info.Properties.TryGetValue("type", out var type))
            {
                type.Write(writer, value);
            }

            if (valueKeys.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var key in valueKeys)
                {
                    var property = info.Properties[key];

                    property.Write(writer, value);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private void ReadResource(ref Utf8JsonReader reader, JsonClassInfo info, object resource, bool attributesRead)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API resource");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                reader.Read();

                if (name == JsonApiMembers.Relationships)
                {
                    ReadRelationships(ref reader, info, resource);
                }
                else if (name == JsonApiMembers.Attributes && !attributesRead)
                {
                    ReadResource(ref reader, info, resource, true);
                }
                else if (!string.IsNullOrEmpty(name) && info.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }
        }

        private void ReadRelationships(ref Utf8JsonReader reader, JsonClassInfo info, object resource)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API relationships");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                reader.Read();

                if (!string.IsNullOrEmpty(name) && info.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, resource);
                }
            }
        }
    }
}
