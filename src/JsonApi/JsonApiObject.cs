using System;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiObject
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(JsonApiVersionConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Version Version { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta Meta { get; set; }
    }
}
