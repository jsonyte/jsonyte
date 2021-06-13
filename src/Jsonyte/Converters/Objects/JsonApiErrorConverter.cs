using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiErrorConverter : WrappedJsonConverter<JsonApiError>
    {
        public override JsonApiError? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonApiError? firstError = null;

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == DocumentFlags.Errors)
                {
                    reader.ReadArray(JsonApiArrayCode.Errors);

                    while (reader.IsInArray())
                    {
                        if (firstError == null)
                        {
                            firstError = ReadWrapped(ref reader, ref tracked, null, options);
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

            return state.HasFlag(DocumentFlags.Errors)
                ? firstError
                : default;
        }

        public override JsonApiError ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonApiError? existingValue, JsonSerializerOptions options)
        {
            var error = new JsonApiError();

            var state = reader.ReadError();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == ErrorFlags.Id)
                {
                    error.Id = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name == ErrorFlags.Links)
                {
                    error.Links = JsonSerializer.Deserialize<JsonApiErrorLinks>(ref reader, options);
                }
                else if (name == ErrorFlags.Status)
                {
                    error.Status = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name == ErrorFlags.Code)
                {
                    error.Code = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name == ErrorFlags.Title)
                {
                    error.Title = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name == ErrorFlags.Detail)
                {
                    error.Detail = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name == ErrorFlags.Source)
                {
                    error.Source = JsonSerializer.Deserialize<JsonApiErrorSource>(ref reader, options);
                }
                else if (name == ErrorFlags.Meta)
                {
                    error.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            return error;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiError value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.ErrorsEncoded);

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
                writer.WriteString(JsonApiMembers.IdEncoded, value.Id);
            }

            if (value.Links != null)
            {
                writer.WritePropertyName(JsonApiMembers.LinksEncoded);
                JsonSerializer.Serialize(writer, value.Links, options);
            }

            if (value.Status != null)
            {
                writer.WriteString(JsonApiMembers.StatusEncoded, value.Status);
            }

            if (value.Code != null)
            {
                writer.WriteString(JsonApiMembers.CodeEncoded, value.Code);
            }

            if (value.Title != null)
            {
                writer.WriteString(JsonApiMembers.TitleEncoded, value.Title);
            }

            if (value.Detail != null)
            {
                writer.WriteString(JsonApiMembers.DetailEncoded, value.Detail);
            }

            if (value.Source != null)
            {
                writer.WritePropertyName(JsonApiMembers.SourceEncoded);
                JsonSerializer.Serialize(writer, value.Source, options);
            }

            if (value.Meta != null)
            {
                writer.WritePropertyName(JsonApiMembers.MetaEncoded);
                JsonSerializer.Serialize(writer, value.Meta, options);
            }

            writer.WriteEndObject();
        }
    }
}
