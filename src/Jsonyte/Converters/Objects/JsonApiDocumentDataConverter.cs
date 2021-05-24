using System.Collections;
using System.Linq;
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

        protected override void ReadErrors(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Errors = ReadWrapped<JsonApiError[]>(ref reader, ref tracked, options);
        }

        protected override void ReadJsonApi(ref Utf8JsonReader reader, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.JsonApi = JsonSerializer.Deserialize<JsonApiObject>(ref reader, options);
        }

        protected override void ReadMeta(ref Utf8JsonReader reader, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
        }

        protected override void ReadLinks(ref Utf8JsonReader reader, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Links = JsonSerializer.Deserialize<JsonApiDocumentLinks>(ref reader, options);
        }

        protected override void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument document, JsonSerializerOptions options)
        {
            document.Included = JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
        }

        protected override void WriteData(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Errors == null && value.Meta == null && value.Data == null)
            {
                writer.WriteNull(JsonApiMembers.DataEncoded);
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                JsonSerializer.Serialize(writer, value.Data, options);
            }
        }

        protected override void WriteErrors(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Errors != null)
            {
                writer.WritePropertyName(JsonApiMembers.ErrorsEncoded);
                WriteWrapped(writer, ref tracked, value.Errors, options);
            }
        }

        protected override void WriteLinks(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.LinksEncoded, value.Links, options);
        }

        protected override void WriteMeta(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.MetaEncoded, value.Meta, options);
        }

        protected override void WriteJsonApi(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.JsonApiEncoded, value.JsonApi, options);
        }

        protected override void WriteIncluded(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument value, JsonSerializerOptions options)
        {
            if (value.Included != null)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);

                writer.WriteStartArray();

                foreach (var resource in value.Included)
                {
                    if (string.IsNullOrEmpty(resource.Id) || string.IsNullOrEmpty(resource.Type))
                    {
                        throw new JsonApiFormatException("JSON:API included resource must contain both 'id' and 'type' members");
                    }

                    JsonSerializer.Serialize(writer, resource, options);
                }

                writer.WriteEndArray();
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
            document.Data = ReadWrapped<T>(ref reader, ref tracked, options);
        }

        protected override void ReadErrors(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.Errors = ReadWrapped<JsonApiError[]>(ref reader, ref tracked, options);
        }

        protected override void ReadJsonApi(ref Utf8JsonReader reader, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.JsonApi = JsonSerializer.Deserialize<JsonApiObject>(ref reader, options);
        }

        protected override void ReadMeta(ref Utf8JsonReader reader, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
        }

        protected override void ReadLinks(ref Utf8JsonReader reader, JsonApiDocument<T> document, JsonSerializerOptions options)
        {
            document.Links = JsonSerializer.Deserialize<JsonApiDocumentLinks>(ref reader, options);
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
                writer.WriteNull(JsonApiMembers.DataEncoded);
            }
            else if (value.Data != null)
            {
                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                WriteWrapped(writer, ref tracked, value.Data, options);
            }
        }

        protected override void WriteErrors(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            if (value.Errors != null)
            {
                writer.WritePropertyName(JsonApiMembers.ErrorsEncoded);
                WriteWrapped(writer, ref tracked, value.Errors, options);
            }
        }

        protected override void WriteLinks(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.LinksEncoded, value.Links, options);
        }

        protected override void WriteMeta(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.MetaEncoded, value.Meta, options);
        }

        protected override void WriteJsonApi(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            WriteIfNotNull(writer, JsonApiMembers.JsonApiEncoded, value.JsonApi, options);
        }

        protected override void WriteIncluded(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiDocument<T> value, JsonSerializerOptions options)
        {
            if (tracked.Count > 0)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                writer.WriteStartArray();

                var elements = GetDataAsList(value.Data);

                if (elements != null)
                {
                    WriteIncludedResourceCollection(writer, ref tracked, elements, options);
                }
                else
                {
                    WriteIncludedResource(writer, ref tracked, value.Data, options);
                }

                writer.WriteEndArray();
            }
        }

        private void WriteIncludedResource(Utf8JsonWriter writer, ref TrackedResources tracked, T? data, JsonSerializerOptions options)
        {
            var index = 0;

            while (index < tracked.Count)
            {
                var included = tracked.Get(index);

                if (!ReferenceEquals(data, included.Value))
                {
                    included.Converter.Write(writer, ref tracked, included.Value, options);
                }

                index++;
            }
        }

        private void WriteIncludedResourceCollection(Utf8JsonWriter writer, ref TrackedResources tracked, IList elements, JsonSerializerOptions options)
        {
            var index = 0;

            while (index < tracked.Count)
            {
                var included = tracked.Get(index);

                var emitIncluded = true;

                foreach (var element in elements)
                {
                    if (ReferenceEquals(element, included.Value))
                    {
                        emitIncluded = false;
                        break;
                    }
                }

                if (emitIncluded)
                {
                    included.Converter.Write(writer, ref tracked, included.Value, options);
                }

                index++;
            }
        }

        private IList? GetDataAsList(T? resources)
        {
            var enumerable = resources as IEnumerable;

            return enumerable switch
            {
                object[] array => array,
                IList list => list,
                null => null,
                _ => enumerable.Cast<object>().ToArray()
            };
        }
    }
}
