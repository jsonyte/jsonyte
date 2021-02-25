using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiPointerConverter : JsonConverter<JsonApiPointer>
    {
        public override JsonApiPointer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonApiException($"Invalid JSON pointer [RFC6901] value: '{value}'");
            }

            return new JsonApiPointer(value);
        }

        public override void Write(Utf8JsonWriter writer, JsonApiPointer value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
