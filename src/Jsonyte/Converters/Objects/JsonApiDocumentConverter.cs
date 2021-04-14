using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Objects
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
            var type = value == null
                ? typeof(TElement)
                : value.GetType();

            var converter = options.GetConverter(type);

            if (converter is WrappedJsonConverter<TElement> genericConverter)
            {
                genericConverter.WriteWrapped(writer, ref tracked, value, options);
            }
            else if (converter is IWrappedObjectConverter anonymousConverter)
            {
                anonymousConverter.WriteWrappedObject(writer, ref tracked, value, options);
            }
            else
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(TElement)}'");
            }
        }

        [AssertionMethod]
        protected abstract void ValidateDocument(T document);
    }
}
