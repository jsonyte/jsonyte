using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiObject
    {
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; } = "1.0";

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
