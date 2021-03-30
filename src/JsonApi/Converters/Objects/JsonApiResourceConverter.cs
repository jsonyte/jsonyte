using System;
using System.Linq;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiResourceConverter<T> : JsonApiConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = default(T);

            var state = reader.ReadDocument();

            while (reader.IsInObject())
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

            var state = reader.ReadResource();

            var info = options.GetClassInfo(typeToConvert);
            var resource = info.Creator();

            ValidateResource(info);

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                var property = info.GetMember(name);

                if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject("resource attributes");

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember("resource object");

                        info.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else
                {
                    property.Read(ref reader, resource);
                }

                reader.Read();
            }

            state.Validate();

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

            ValidateResource(info);

            var comparer = options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            var valueKeys = info.GetMemberKeys()
                .Except(new[] {JsonApiMembers.Id, JsonApiMembers.Type, JsonApiMembers.Meta}, comparer)
                .ToArray();

            writer.WriteStartObject();

            info.GetMember(JsonApiMembers.Id).Write(writer, value);
            info.GetMember(JsonApiMembers.Type).Write(writer, value);
            
            if (valueKeys.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var key in valueKeys)
                {
                    info.GetMember(key).Write(writer, value);
                }

                writer.WriteEndObject();
            }

            info.GetMember(JsonApiMembers.Meta).Write(writer, value);

            writer.WriteEndObject();
        }

        protected void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.GetMember(JsonApiMembers.Id);

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource id must be a string");
            }

            var typeProperty = info.GetMember(JsonApiMembers.Type);

            if (string.IsNullOrEmpty(typeProperty.Name))
            {
                throw new JsonApiException("JSON:API resource must have a 'type' member");
            }

            if (typeProperty.MemberType != typeof(string))
            {
                throw new JsonApiException("JSON:API resource type must be a string");
            }
        }
    }
}
