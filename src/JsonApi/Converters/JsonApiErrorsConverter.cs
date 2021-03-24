using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiErrorsConverter<T> : JsonApiConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var errors = default(T);

            var state = reader.ReadDocument();

            while (reader.IsObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Errors)
                {
                    errors = ReadWrapped(ref reader, typeToConvert, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return state.HasFlag(JsonApiDocumentFlags.Errors)
                ? errors
                : default;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var errors = new List<JsonApiError>();
            var converter = options.GetWrappedConverter<JsonApiError>();

            reader.ReadArray("errors");

            while (reader.IsArray())
            {
                var error = converter.ReadWrapped(ref reader, typeof(JsonApiError), options);

                if (error != null)
                {
                    errors.Add(error);
                }

                reader.Read();
            }

            var items = GetCollection(typeToConvert, errors);

            return items == null
                ? default
                : (T) items;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("errors");

            WriteWrapped(writer, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value is IEnumerable<JsonApiError> errors)
            {
                var converter = options.GetWrappedConverter<JsonApiError>();

                foreach (var error in errors)
                {
                    if (error == null)
                    {
                        throw new JsonApiException("Invalid JSON:API errors, error item cannot be null");
                    }

                    converter.WriteWrapped(writer, error, options);
                }
            }

            writer.WriteEndArray();
        }

        private object? GetCollection(Type typeToConvert, List<JsonApiError>? errors)
        {
            if (errors == null)
            {
                return null;
            }

            if (typeToConvert.GetTypeCategory() == JsonTypeCategory.Array)
            {
                return errors.ToArray();
            }

            return errors;
        }
    }
}
