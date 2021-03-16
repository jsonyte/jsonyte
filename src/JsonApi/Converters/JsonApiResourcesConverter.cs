using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourcesConverter : JsonConverter<JsonApiResource[]>
    {
        public override JsonApiResource[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException("Expected single resource or array of resources for this data");
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var resource = reader.Read<JsonApiResource>(options);

                return new[] {resource};
            }

            return reader.Read<JsonApiResource[]>(options);
        }

        public override void Write(Utf8JsonWriter writer, JsonApiResource[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
