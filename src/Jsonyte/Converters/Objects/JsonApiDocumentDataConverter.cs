using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiDocumentDataConverter : JsonApiDocumentConverter<JsonApiDocument>
    {
        protected override void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Data = reader.Read<JsonApiResource[]>(options);
        }

        protected override void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Included = JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
        }

        protected override void WriteData(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Errors == null && value.Meta == null && value.Data == null)
            {
                writer.WriteNull(JsonApiMembers.Data);
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                JsonSerializer.Serialize(writer, value.Data, options);
            }
        }

        protected override void WriteIncluded(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Included != null)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                JsonSerializer.Serialize(writer, value.Included, options);
            }
        }

        protected override void ValidateDocument(JsonApiDocument document)
        {
            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiFormatException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (document.Data == null && document.Included != null)
            {
                throw new JsonApiFormatException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }
    }

    internal class JsonApiDocumentDataConverter<T> : JsonApiDocumentConverter<JsonApiDocument<T>>
    {
        protected override void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.Data = reader.ReadWrapped<T>(ref tracked, options);
        }

        protected override void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            reader.ReadArray(JsonApiArrayCode.Included);

            while (reader.IsInArray())
            {
                var identifier = reader.ReadAheadIdentifier();

                if (tracked.TryGetIncluded(identifier, out var included))
                {
                    included.Converter.Read(ref reader, ref tracked, included.Value, options);
                }
                else
                {
                    throw new JsonApiFormatException("JSON:API included resource must be referenced by at least one relationship");
                }

                reader.Read();
            }
        }

        protected override void ValidateDocument(JsonApiDocument<T> document)
        {
            if (document.Data != null && document.Errors != null)
            {
                throw new JsonApiFormatException("JSON:API document must not contain both 'data' and 'errors' members");
            }
        }

        protected override void WriteData(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            if (value.Errors == null && value.Meta == null && value.Data == null)
            {
                writer.WriteNull(JsonApiMembers.Data);
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                WriteWrapped(writer, ref tracked, value.Data, options);
            }
        }

        protected override void WriteIncluded(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            if (tracked.Count > 0)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                writer.WriteStartArray();

                var index = 0;

                while (index < tracked.Count)
                {
                    var included = tracked.Get(index);
                    included.Converter.Write(writer, ref tracked, included.Value, options);

                    index++;
                }

                writer.WriteEndArray();
            }
        }
    }
}
