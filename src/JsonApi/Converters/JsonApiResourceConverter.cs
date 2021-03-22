using System;
using System.Linq;
using System.Text.Json;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConverter<T> : JsonApiConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = default(T);

            var state = reader.ReadDocument();

            while (reader.IsObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resource = ReadWrapped(ref reader, typeToConvert, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadObject("resource");

            var info = options.GetClassInfo(typeToConvert);
            var resource = info.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsObject())
            {
                var name = reader.ReadMember("resource object");

                if (name == JsonApiMembers.Id || name == JsonApiMembers.Type || name == JsonApiMembers.Meta || name == JsonApiMembers.Links)
                {
                    if (info.Properties.TryGetValue(name, out var property))
                    {
                        property.Read(ref reader, resource);
                    }
                }
                else if (name == JsonApiMembers.Relationships)
                {
                }
                else if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject("resource attributes");

                    while (reader.IsObject())
                    {
                        var attribute = reader.ReadMember("resource object");

                        if (attribute != null && info.Properties.TryGetValue(attribute, out var property))
                        {
                            property.Read(ref reader, resource);
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

            return (T) resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("data");

            WriteWrapped(writer, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();

                return;
            }

            var info = options.GetClassInfo(typeof(T));

            var valueKeys = info.Properties.Keys
                .Except(new[] {JsonApiMembers.Id, JsonApiMembers.Type})
                .ToArray();

            writer.WriteStartObject();

            if (info.Properties.TryGetValue(JsonApiMembers.Id, out var idProperty))
            {
                idProperty.Write(writer, value);
            }

            if (info.Properties.TryGetValue(JsonApiMembers.Type, out var typeProperty))
            {
                typeProperty.Write(writer, value);
            }

            if (valueKeys.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var key in valueKeys)
                {
                    var property = info.Properties[key];

                    property.Write(writer, value);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }

#if false
    internal class JsonApiResourceConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiResourceDocument<T>>(ref reader, options);

                if (document == null)
                {
                    throw new JsonApiException("Invalid JSON:API document");
                }

                return document.Data;
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var info = options.GetClassInfo(typeof(T));
            var resource = info.Creator();

            ReadResource(ref reader, info, resource, false);

            return (T) resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (writer.CurrentDepth == 0)
            {
                var document = new JsonApiDocument<T>
                {
                    Data = value
                };

                JsonSerializer.Serialize(writer, document, options);

                return;
            }

            writer.WriteStartObject();

            var info = options.GetClassInfo(typeof(T));

            var valueKeys = info.Properties.Keys
                .Except(new[] {JsonApiMembers.Id, JsonApiMembers.Type})
                .ToArray();

            if (info.Properties.TryGetValue("id", out var id))
            {
                id.Write(writer, value);
            }

            if (info.Properties.TryGetValue("type", out var type))
            {
                type.Write(writer, value);
            }

            if (valueKeys.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var key in valueKeys)
                {
                    var property = info.Properties[key];

                    property.Write(writer, value);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private void ReadResource(ref Utf8JsonReader reader, JsonClassInfo info, object resource, bool attributesRead)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API resource");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                reader.Read();

                if (name == JsonApiMembers.Relationships)
                {
                    ReadRelationships(ref reader, info, resource);
                }
                else if (name == JsonApiMembers.Attributes && !attributesRead)
                {
                    ReadResource(ref reader, info, resource, true);
                }
                else if (!string.IsNullOrEmpty(name) && info.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }
        }

        private void ReadRelationships(ref Utf8JsonReader reader, JsonClassInfo info, object resource)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API relationships");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonApiException($"Expected top-level JSON:API property name but found '{reader.GetString()}'");
                }

                var name = reader.GetString();

                reader.Read();

                if (!string.IsNullOrEmpty(name) && info.Properties.TryGetValue(name, out var property))
                {
                    property.Read(ref reader, resource);
                }
            }
        }
    }
#endif
}
