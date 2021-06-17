using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithLinks
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("links")]
        public JsonApiResourceLinks Links { get; set; }
    }
}
