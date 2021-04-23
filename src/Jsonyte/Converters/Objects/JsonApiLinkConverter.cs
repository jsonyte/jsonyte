using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiLinkConverter : JsonConverter<JsonApiLink>
    {
        public override JsonApiLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var link = new JsonApiLink();

            if (reader.TokenType == JsonTokenType.String)
            {
                link.Href = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read();

                while (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var key = reader.ValueSpan.GetKey();

                    reader.Read();

                    if (key == JsonApiMembers.HrefKey)
                    {
                        link.Href = reader.GetString();
                    }
                    else if (key == JsonApiMembers.MetaKey)
                    {
                        link.Meta = JsonSerializer.Deserialize<JsonApiMeta>(ref reader, options);
                    }
                    else
                    {
                        reader.Skip();

                    }

                    reader.Read();
                }
            }

            return link;
        }

        public override void Write(Utf8JsonWriter writer, JsonApiLink value, JsonSerializerOptions options)
        {
            if (value.Meta == null)
            {
                writer.WriteStringValue(value.Href);
            }
            else
            {
                writer.WriteStartObject();

                writer.WritePropertyName(JsonApiMembers.HrefEncoded);
                writer.WriteStringValue(value.Href);

                writer.WritePropertyName(JsonApiMembers.MetaEncoded);
                JsonSerializer.Serialize(writer, value.Meta, options);

                writer.WriteEndObject();
            }
        }
    }
}
