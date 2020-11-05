using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument<T>>(ref reader, options);

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
            throw new NotImplementedException();
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

                if (!attributesRead && name == JsonApiMembers.Attributes)
                {
                    ReadResource(ref reader, info, resource, true);
                }
                else if (info.Properties.TryGetValue(name, out var property))
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
    }
}
