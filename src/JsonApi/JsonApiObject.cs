using System;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiObject
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(JsonApiVersionConverter))]
        public Version Version { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }
}
