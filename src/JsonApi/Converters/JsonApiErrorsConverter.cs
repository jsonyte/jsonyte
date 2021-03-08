using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiErrorsConverter<T> : JsonConverter<T>
        where T : class
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var state = reader.BeginDocument();

            var list = new List<JsonApiError>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var name = reader.ReadToMember(ref state);

                if (name == JsonApiMembers.Errors)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonApiException("Invalid JSON:API errors array");
                    }

                    var converter = options.GetConverter<JsonApiError>();

                    reader.Read();

                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        var error = converter.ReadWrapped(ref reader, typeof(JsonApiError), options);

                        if (error != null)
                        {
                            list.Add(error);
                        }

                        reader.Read();
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            if (!state.HasFlag(JsonApiDocumentFlags.Errors))
            {
                return default;
            }

            return GetCollection(typeToConvert, list) as T;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("errors");

            writer.WriteStartArray();

            if (value is IEnumerable<JsonApiError> errors)
            {
                var converter = options.GetConverter<JsonApiError>();

                foreach (var error in errors)
                {
                    converter.WriteWrapped(writer, error, options);
                }
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private object? GetCollection(Type typeToConvert, List<JsonApiError>? errors)
        {
            if (errors == null)
            {
                return null;
            }

            if (typeToConvert.GetClassType() == JsonClassType.Array)
            {
                return errors.ToArray();
            }

            return errors;
        }
    }
}
