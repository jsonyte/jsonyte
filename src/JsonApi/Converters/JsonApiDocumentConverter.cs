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
                throw new JsonApiException("Invalid JSON:API document, expected JSON object");
            }

            var type = options.GetClassInfo(typeToConvert);
            var flags = JsonApiDocumentFlags.None;

            var resource = type.Creator();

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();
                //flags = flags.AddFlag(name);

                reader.Read();

                if (!string.IsNullOrEmpty(name) && type.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            //flags.Validate();

            return (T) resource
;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    //internal class JsonApiDocumentConverter<T> : JsonConverter<T>
    //{
    //    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        if (reader.TokenType != JsonTokenType.StartObject)
    //        {
    //            throw new JsonApiException("Invalid JSON:API document");
    //        }

    //        var type = options.GetClassInfo(typeToConvert);

    //        var document = type.Creator();

    //        reader.Read();

    //        var flags = DocumentFlags.None;

    //        while (reader.TokenType != JsonTokenType.EndObject)
    //        {
    //            if (reader.TokenType != JsonTokenType.PropertyName)
    //            {
    //                throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
    //            }

    //            var name = reader.GetString();

    //            AddFlag(ref flags, name);

    //            reader.Read();

    //            if (!string.IsNullOrEmpty(name) && type.Properties.TryGetValue(name, out var property))
    //            {
    //                property.Read(ref reader, document);
    //            }
    //            else
    //            {
    //                reader.Skip();
    //            }

    //            reader.Read();
    //        }

    //        ValidateFlags(flags);

    //        return (T) document;
    //    }

    //    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    //    {
    //        if (value is JsonApiDocument document)
    //        {
    //            ValidateDocument(document);

    //            writer.WriteStartObject();

    //            if (document.Data != null)
    //            {
    //                JsonSerializer.Serialize(writer, document.Data, options);
    //            }
    //            else if (document.Errors == null && document.Meta == null)
    //            {
    //                writer.WriteNull(JsonApiMembers.Data);
    //            }

    //            if (document.Errors != null)
    //            {
    //                JsonSerializer.Serialize(writer, document.Errors, options);
    //            }

    //            if (document.Links != null)
    //            {
    //                writer.WritePropertyName(JsonApiMembers.Links);

    //                JsonSerializer.Serialize(writer, document.Links, options);
    //            }

    //            if (document.Meta != null)
    //            {
    //                JsonSerializer.Serialize(writer, document.Meta, options);
    //            }

    //            writer.WriteEndObject();
    //        }
    //        else if (value is JsonApiDocument<T> typedDocument)
    //        {
    //            ValidateDocument(typedDocument);

    //            writer.WriteStartObject();

    //            if (typedDocument.Data != null)
    //            {
    //                JsonSerializer.Serialize(writer, typedDocument.Data, options);
    //            }

    //            if (typedDocument.Errors != null)
    //            {
    //                JsonSerializer.Serialize(writer, typedDocument.Errors, options);
    //            }

    //            if (typedDocument.Links != null)
    //            {
    //                JsonSerializer.Serialize(writer, typedDocument.Links, options);
    //            }

    //            if (typedDocument.Meta != null)
    //            {
    //                JsonSerializer.Serialize(writer, typedDocument.Meta, options);
    //            }

    //            writer.WriteEndObject();
    //        }
    //    }

    //    private void ValidateDocument(JsonApiDocument document)
    //    {
    //        if (document.Data != null && document.Errors != null)
    //        {
    //            throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
    //        }

    //        if (document.Data == null && document.Included != null)
    //        {
    //            throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
    //        }
    //    }

    //    private void ValidateDocument(JsonApiDocument<T> document)
    //    {
    //        if (document.Data != null && document.Errors != null)
    //        {
    //            throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
    //        }

    //        if (document.Data == null && document.Included != null)
    //        {
    //            throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
    //        }
    //    }
    //}
}
