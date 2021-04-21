using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
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

                if (name.SequenceEqual(JsonApiMembers.ErrorsEncoded.EncodedUtf8Bytes))
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

                if (name.SequenceEqual(JsonApiMembers.IdEncoded.EncodedUtf8Bytes))
                {
                    error.Id = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.LinksEncoded.EncodedUtf8Bytes))
                {
                    error.Links = JsonSerializer.Deserialize<JsonApiErrorLinks>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.StatusEncoded.EncodedUtf8Bytes))
                {
                    error.Status = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.CodeEncoded.EncodedUtf8Bytes))
                {
                    error.Code = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.TitleEncoded.EncodedUtf8Bytes))
                {
                    error.Title = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.DetailEncoded.EncodedUtf8Bytes))
                {
                    error.Detail = JsonSerializer.Deserialize<string>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.SourceEncoded.EncodedUtf8Bytes))
                {
                    error.Source = JsonSerializer.Deserialize<JsonApiErrorSource>(ref reader, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.MetaEncoded.EncodedUtf8Bytes))
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
