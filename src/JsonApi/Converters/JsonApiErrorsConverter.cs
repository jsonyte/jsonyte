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
            throw new NotImplementedException();
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

            if (info.ClassType == JsonClassType.List)
            {
                return errors;
            }

            return errors;
        }
    }
}
