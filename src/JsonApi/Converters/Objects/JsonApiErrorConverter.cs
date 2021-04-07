using System;
using System.Text.Json;
using JsonApi.Serialization;
using JsonApi.Validation;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiErrorConverter : WrappedJsonConverter<JsonApiError>
    {
        public Type TypeToConvert { get; } = typeof(JsonApiError);

        public override JsonApiError? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonApiError? firstError = null;

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Errors)
                {
                    reader.ReadArray(JsonApiArrayCode.Errors);

                    while (reader.IsInArray())
                    {
                        if (firstError == null)
                        {
                            firstError = ReadWrapped(ref reader, ref tracked, TypeToConvert!, null, options);
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

            tracked.Release();

            state.Validate();

            return state.HasFlag(DocumentFlags.Errors)
                ? firstError
                : default;
        }

        public override JsonApiError ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonApiError? existingValue, JsonSerializerOptions options)
        {
            var error = new JsonApiError();

            reader.ReadObject(JsonApiMemberCode.Error);

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(JsonApiMemberCode.Error);

                switch (name)
                {
                    case JsonApiMembers.Id:
                        error.Id = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case JsonApiMembers.Links:
                        error.Links = JsonSerializer.Deserialize<JsonApiErrorLinks>(ref reader, options);
                        break;

                    case JsonApiMembers.Status:
                        error.Status = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case JsonApiMembers.Code:
                        error.Code = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case JsonApiMembers.Title:
                        error.Title = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case JsonApiMembers.Detail:
                        error.Detail = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case JsonApiMembers.Source:
                        error.Source = JsonSerializer.Deserialize<JsonApiErrorSource>(ref reader, options);
                        break;

                    case JsonApiMembers.Meta:
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
            var tracked = new TrackedResources();

            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.Errors);

            writer.WriteStartArray();
            WriteWrapped(writer, ref tracked, value, options);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, JsonApiError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.Id != null)
            {
                writer.WriteString(JsonApiMembers.Id, value.Id);
            }

            if (value.Links != null)
            {
                writer.WritePropertyName(JsonApiMembers.Links);
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Status != null)
            {
                writer.WriteString(JsonApiMembers.Status, value.Status);
            }

            if (value.Code != null)
            {
                writer.WriteString(JsonApiMembers.Code, value.Code);
            }

            if (value.Title != null)
            {
                writer.WriteString(JsonApiMembers.Title, value.Title);
            }

            if (value.Detail != null)
            {
                writer.WriteString(JsonApiMembers.Detail, value.Detail);
            }

            if (value.Source != null)
            {
                writer.WritePropertyName(JsonApiMembers.Source);
                JsonSerializer.Serialize(writer, value.Source, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName(JsonApiMembers.Meta);
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            writer.WriteEndObject();
        }
    }
}
