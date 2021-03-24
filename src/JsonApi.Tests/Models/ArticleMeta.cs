using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class ArticleMeta
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
