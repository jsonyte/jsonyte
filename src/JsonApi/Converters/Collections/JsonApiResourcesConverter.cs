using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiResourcesConverter : JsonConverter<JsonApiResource[]>
    {
        public override JsonApiResource[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (!reader.IsObject() && !reader.IsArray())
            {
                throw new JsonApiException("Expected single JSON:API resource or array of resources for this data");
            }

            if (reader.IsObject())
            {
                var resource = reader.Read<JsonApiResource>(options);

                if (resource == null)
                {
                    throw new JsonApiException("JSON:API resource must not be empty");
                }

                return new[] {resource};
            }

            var resources = new List<JsonApiResource>();

            reader.ReadArray("resources");

            while (reader.IsInArray())
            {
                var resource = reader.Read<JsonApiResource>(options);

                if (resource == null)
                {
                    throw new JsonApiException("JSON:API resource must not be empty");
                }

                resources.Add(resource);

                reader.Read();
            }

            return resources.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiResource[] value, JsonSerializerOptions options)
        {
            if (value.Length == 1)
            {
                JsonSerializer.Serialize(writer, value[0], options);
            }
            else
            {
                writer.WriteStartArray();

                foreach (var resource in value)
                {
                    JsonSerializer.Serialize(writer, resource, options);
                }

                writer.WriteEndArray();
            }
        }
    }
}
