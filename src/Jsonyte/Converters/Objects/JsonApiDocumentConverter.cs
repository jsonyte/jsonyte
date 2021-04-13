using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using JsonApi.Serialization;

namespace JsonApi.Converters.Objects
{
    internal abstract class JsonApiDocumentConverter<T> : JsonConverter<T>
        where T : IJsonApiDocument, new()
    {
        protected abstract void ReadData(ref Utf8JsonReader reader, ref TrackedResources tracked, T document, JsonSerializerOptions options);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = new T();

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    ReadData(ref reader, ref tracked, document, options);
                }
                else if (name == JsonApiMembers.Errors)
                {
                    document.Errors = reader.ReadWrapped<JsonApiError[]>(ref tracked, options);
                }
                else if (name == JsonApiMembers.JsonApi)
                {
                    document.JsonApi = JsonSerializer.Deserialize<JsonApiObject>(ref reader, options);
                }
                else if (name == JsonApiMembers.Meta)
                {
                    document.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
                }
                else if (name == JsonApiMembers.Links)
                {
                    document.Links = JsonSerializer.Deserialize<JsonApiDocumentLinks>(ref reader, options);
                }
                else if (name == JsonApiMembers.Included)
                {
                    document.Included = JsonSerializer.Deserialize<JsonApiResource[]>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return state.IsEmpty()
                ? default
                : document;
        }

        protected abstract void WriteData(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            ValidateDocument(value);

            writer.WriteStartObject();

            WriteData(writer, ref tracked, value, options);

            if (value.Included != null)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                JsonSerializer.Serialize(writer, value.Included, options);
            }

            if (value.Errors != null)
            {
                writer.WritePropertyName(JsonApiMembers.ErrorsEncoded);
                WriteWrapped(writer, ref tracked, value.Errors, options);
            }

            if (value.Links != null)
            {
                writer.WritePropertyName(JsonApiMembers.LinksEncoded);
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName(JsonApiMembers.MetaEncoded);
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            if (value.JsonApi != null)
            {
                writer.WritePropertyName(JsonApiMembers.JsonApiEncoded);
                JsonSerializer.Serialize(writer, value.JsonApi, options);
            }

            writer.WriteEndObject();
        }

        protected void WriteWrapped<TElement>(Utf8JsonWriter writer, ref TrackedResources tracked, TElement value, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(TElement)) is not WrappedJsonConverter<TElement> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(TElement)}'");
            }

            converter.WriteWrapped(writer, ref tracked, value, options);
        }

        [AssertionMethod]
        protected abstract void ValidateDocument(T document);
    }
}
