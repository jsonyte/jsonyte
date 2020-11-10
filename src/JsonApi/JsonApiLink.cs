using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    [JsonConverter(typeof(JsonApiLinkConverter))]
    public class JsonApiLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }
}
