using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleMeta
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
