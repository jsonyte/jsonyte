using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters.Objects
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
                    var name = reader.GetString();

                    reader.Read();

                    if (name == JsonApiMembers.Href)
                    {
                        link.Href = reader.GetString();
                    }
                    else if (name == JsonApiMembers.Meta)
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

                writer.WritePropertyName(JsonApiMembers.Href);
                writer.WriteStringValue(value.Href);

                writer.WritePropertyName(JsonApiMembers.Meta);
                JsonSerializer.Serialize(writer, value.Meta, options);

                writer.WriteEndObject();
            }
        }
    }
}
