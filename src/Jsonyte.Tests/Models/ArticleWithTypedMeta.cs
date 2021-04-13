using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class ArticleWithTypedMeta
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("meta")]
        public ArticleMeta Meta { get; set; }
    }
}
