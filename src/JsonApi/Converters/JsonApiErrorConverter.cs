using System;
using System.Text.Json;

namespace JsonApi.Converters
{
    internal class JsonApiErrorConverter : JsonApiConverter<JsonApiError>
    {
        public override JsonApiError? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var state = reader.BeginDocument();

            JsonApiError? firstError = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var name = reader.ReadToMember(ref state);

                if (name == JsonApiMembers.Errors)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonApiException("Invalid JSON:API errors array, expected array");
                    }

                    reader.Read();

                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (firstError == null)
                        {
                            firstError = ReadWrapped(ref reader, typeof(JsonApiError), options);
                        }
                        else
                        {
                            reader.Skip();
                        }
                        
                        reader.Read();
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            if (!state.HasFlag(JsonApiDocumentFlags.Errors))
            {
                return default;
            }

            return firstError;
        }

        public override JsonApiError ReadWrapped(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API error object, expected JSON object");
            }

            var error = new JsonApiError();

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected JSON:API error object property but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                switch (name)
                {
                    case "id":
                        error.Id = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "links":
                        error.Links = JsonSerializer.Deserialize<JsonApiErrorLinks>(ref reader, options);
                        break;

                    case "status":
                        error.Status = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "code":
                        error.Code = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "title":
                        error.Title = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "detail":
                        error.Detail = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "source":
                        error.Source = JsonSerializer.Deserialize<JsonApiErrorSource>(ref reader, options);
                        break;

                    case "meta":
                        error.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
                        break;

                    default:
                        reader.Skip();
                        break;
                }

                reader.Read();
            }

            return error;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("errors");

            writer.WriteStartArray();
            WriteWrapped(writer, value, options);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, JsonApiError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.Id != null)
            {
                writer.WriteString("id", value.Id);
            }

            if (value.Links != null)
            {
                writer.WritePropertyName("links");
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Status != null)
            {
                writer.WriteString("status", value.Status);
            }

            if (value.Code != null)
            {
                writer.WriteString("code", value.Code);
            }

            if (value.Title != null)
            {
                writer.WriteString("title", value.Title);
            }

            if (value.Detail != null)
            {
                writer.WriteString("detail", value.Detail);
            }

            if (value.Source != null)
            {
                writer.WritePropertyName("source");
                JsonSerializer.Serialize(writer, value.Source, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName("meta");
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            writer.WriteEndObject();
        }
    }
}
