using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document");
            }

            var type = options.GetClassInfo(typeToConvert);

            var document = type.Creator();

            reader.Read();

            var flags = DocumentFlags.None;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                AddFlag(ref flags, name);

                reader.Read();

                if (!string.IsNullOrEmpty(name) && type.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, document);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            ValidateFlags(flags);

            return (T) document;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is JsonApiDocument document)
            {
                ValidateDocument(document);

                writer.WriteStartObject();

                if (document.Errors != null)
                {
                    JsonSerializer.Serialize(writer, document.Errors, options);
                }

                writer.WriteEndObject();
            }
            else if (value is JsonApiDocument<T> typedDocument)
            {
                ValidateDocument(typedDocument);

                writer.WriteStartObject();

                if (typedDocument.Data != null)
                {
                    JsonSerializer.Serialize(writer, typedDocument.Data, options);
                }

                writer.WriteEndObject();
            }
        }

        private void AddFlag(ref DocumentFlags flags, string name)
        {
            switch (name)
            {
                case JsonApiMembers.Data:
                    flags |= DocumentFlags.Data;
                    break;

                case JsonApiMembers.Errors:
                    flags |= DocumentFlags.Errors;
                    break;

                case JsonApiMembers.Meta:
                    flags |= DocumentFlags.Meta;
                    break;

                case JsonApiMembers.Version:
                    flags |= DocumentFlags.Jsonapi;
                    break;

                case JsonApiMembers.Links:
                    flags |= DocumentFlags.Links;
                    break;

                case JsonApiMembers.Included:
                    flags |= DocumentFlags.Included;
                    break;
            }
        }

        private void ValidateFlags(DocumentFlags flags)
        {
            if (!flags.HasFlag(DocumentFlags.Data) &&
                !flags.HasFlag(DocumentFlags.Errors) &&
                !flags.HasFlag(DocumentFlags.Meta))
            {
                throw new JsonApiException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (flags.HasFlag(DocumentFlags.Data) && flags.HasFlag(DocumentFlags.Errors))
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (flags.HasFlag(DocumentFlags.Included) && !flags.HasFlag(DocumentFlags.Data))
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        private void ValidateDocument(JsonApiDocument document)
        {
            if (document.Data == null && document.Errors == null && document.Meta == null)
            {
                throw new JsonApiException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (document.Data == null && document.Included != null)
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        private void ValidateDocument(JsonApiDocument<T> document)
        {
            if (document.Data == null && document.Errors == null && document.Meta == null)
            {
                throw new JsonApiException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (document.Data == null && document.Included != null)
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }
    }
}
