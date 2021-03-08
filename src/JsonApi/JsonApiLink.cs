using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public class JsonApiLink
    {
        public static implicit operator JsonApiLink(string href)
        {
            return new()
            {
                Href = href
            };
        }

        [JsonPropertyName("href")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Href { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
