using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiVersionConverter : JsonConverter<Version>
    {
        private static readonly Version MinimumVersion = Version.Parse("1.0");

        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrEmpty(value) || !Version.TryParse(value, out var version))
            {
                throw new JsonApiException($"Invalid JSON:API version: '{value}'");
            }

            if (version < MinimumVersion)
            {
                throw new JsonApiException("JSON:API version is before than '1.0', minimum required version is '1.0'");
            }

            return version;
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
