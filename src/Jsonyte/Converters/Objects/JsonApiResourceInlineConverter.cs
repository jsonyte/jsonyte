using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiResourceInlineConverter<T> : JsonConverter<InlineResource<T>>
    {
        private JsonTypeInfo? info;

        public override InlineResource<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            EnsureTypeInfo(options);

            reader.ReadResource();

            var resource = info!.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var attributeName = reader.ReadMember(JsonApiMemberCode.Resource);

                info!.GetMember(attributeName).Read(ref reader, resource);

                reader.Read();
            }

            return new InlineResource<T>((T) resource);
        }

        public override void Write(Utf8JsonWriter writer, InlineResource<T> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        private void EnsureTypeInfo(JsonSerializerOptions options)
        {
            info ??= options.GetTypeInfo(typeof(T));
        }
    }
}
