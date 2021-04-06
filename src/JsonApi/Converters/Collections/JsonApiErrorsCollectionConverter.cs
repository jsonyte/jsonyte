using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;
using JsonApi.Validation;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiErrorsCollectionConverter<T> : WrappedJsonConverter<T>
    {
        public Type? ElementType { get; } = typeof(JsonApiError);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var errors = default(T);

            var state = reader.ReadDocument();
            var readState = new TrackedResources();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Errors)
                {
                    errors = ReadWrapped(ref reader, ref readState, typeToConvert, default, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return state.HasFlag(DocumentFlags.Errors)
                ? errors
                : default;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            var errors = new List<JsonApiError>();
            var converter = options.GetWrappedConverter<JsonApiError>();

            reader.ReadArray("errors");

            while (reader.IsInArray())
            {
                var error = converter.ReadWrapped(ref reader, ref tracked, ElementType!, null, options);

                if (error == null)
                {
                    throw new JsonApiException("JSON:API error object must not be null");
                }

                errors.Add(error);

                reader.Read();
            }

            var items = GetCollection(errors);

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

        private object? GetCollection(List<JsonApiError>? errors)
        {
            if (errors == null)
            {
                return null;
            }

            return TypeCategory == JsonTypeCategory.Array
                ? errors.ToArray()
                : errors;
        }
    }
}
