using System.Text.Json;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiDocumentDataConverter : JsonApiDocumentConverter<JsonApiDocument>
    {
        protected override void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Data = reader.Read<JsonApiResource[]>(options);
        }

        protected override void ValidateDocument(JsonApiDocument document)
        {
            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (document.Data == null && document.Included != null)
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        protected override void WriteData(Utf8JsonWriter writer, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Errors == null && value.Meta == null && value.Data == null)
            {
                writer.WriteNull("data");
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName("data");
                writer.Write(value.Data, options);
            }
        }
    }

    internal class JsonApiDocumentDataConverter<T> : JsonApiDocumentConverter<JsonApiDocument<T>>
    {
        protected override void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.Data = reader.ReadWrapped<T>(ref tracked, options);
        }

        protected override void ValidateDocument(JsonApiDocument<T> document)
        {
            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (document.Data == null && document.Included != null)
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        protected override void WriteData(Utf8JsonWriter writer, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            if (value.Errors == null && value.Meta == null && value.Data == null)
            {
                writer.WriteNull("data");
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName("data");
                writer.WriteWrapped(value.Data, options);
            }
        }
    }
}
