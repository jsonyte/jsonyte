using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiResourceIdentifier
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }
}
