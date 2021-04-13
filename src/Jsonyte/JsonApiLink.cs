using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiLink
    {
        public static implicit operator JsonApiLink(string href)
        {
            return new()
            {
                Href = href
            };
        }

        public static implicit operator string?(JsonApiLink value)
        {
            return value.Href;
        }

        [JsonPropertyName("href")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Href { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
