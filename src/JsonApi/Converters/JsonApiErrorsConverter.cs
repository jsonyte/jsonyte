using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiErrorsConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = options.GetClassInfo(typeToConvert);

            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument>(ref reader, options);

                return (T) GetInstance(type, document?.Errors?.ToList());
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException("Invalid JSON:API errors array");
            }

            reader.Read();

            var list = new List<JsonApiError>();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var error = JsonSerializer.Deserialize<JsonApiError>(ref reader, options);

                list.Add(error);

                reader.Read();
            }

            return (T) GetInstance(type, list);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var errors = GetErrors(value);

            if (writer.CurrentDepth == 0)
            {
                var document = new JsonApiDocument
                {
                    Errors = errors
                };

                JsonSerializer.Serialize(writer, document, options);

                return;
            }

            writer.WritePropertyName("errors");
            writer.WriteStartArray();

            foreach (var error in errors)
            {
                JsonSerializer.Serialize(writer, error, options);
            }

            writer.WriteEndArray();
        }

        private object GetInstance(JsonClassInfo info, List<JsonApiError> errors)
        {
            if (errors == null)
            {
                return null;
            }

            if (info.ClassType == JsonClassType.Array)
            {
                return errors.ToArray();
            }

            return errors;
        }

        private JsonApiError[] GetErrors(T value)
        {
            if (value is JsonApiError[] array)
            {
                return array;
            }

            if (!(value is IEnumerable<JsonApiError> enumerable))
            {
                throw new JsonApiException("Expected array or list type of JsonApiError objects");
            }

            return enumerable.ToArray();
        }
    }
}
