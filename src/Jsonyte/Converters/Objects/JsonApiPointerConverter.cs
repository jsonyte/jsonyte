using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiPointerConverter : JsonConverter<JsonApiPointer>
    {
        public override JsonApiPointer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonApiFormatException($"Invalid JSON pointer [RFC6901] value: '{value}'");
            }

            return new JsonApiPointer(value ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, JsonApiPointer value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
