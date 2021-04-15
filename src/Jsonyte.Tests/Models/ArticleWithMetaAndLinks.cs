using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithMetaAndLinks
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("links")]
        public Dictionary<string, string> Links { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, object> Meta { get; set; }
    }
}
