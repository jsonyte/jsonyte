using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace JsonApi.Converters
{
    internal abstract class JsonApiDocumentBaseConverter<T> : JsonConverter<T>
        where T : IJsonApiDocument, new()
    {
        protected abstract void ReadData(ref Utf8JsonReader reader, T document, JsonSerializerOptions options);

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = new T();

            var state = reader.ReadDocument();

            while (reader.IsObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    ReadData(ref reader, document, options);
                }
                else if (name == JsonApiMembers.Errors)
                {
                    document.Errors = reader.ReadWrapped<JsonApiError[]>(options);
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
                    document.Links = JsonSerializer.Deserialize<JsonApiLinks>(ref reader, options);
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

        protected abstract void WriteData(Utf8JsonWriter writer, T value, JsonSerializerOptions options);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            ValidateDocument(value);

            writer.WriteStartObject();

            WriteData(writer, value, options);

            if (value.Errors != null)
            {
                writer.WritePropertyName("errors");
                writer.WriteWrapped(value.Errors, options);
            }

            if (value.Links != null)
            {
                writer.WritePropertyName("links");
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName("meta");
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            if (value.JsonApi != null)
            {
                writer.WritePropertyName("jsonapi");
                JsonSerializer.Serialize(writer, value.JsonApi, options);
            }

            writer.WriteEndObject();
        }

        [AssertionMethod]
        protected abstract void ValidateDocument(T document);
    }
}
