using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithMeta
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }
}
