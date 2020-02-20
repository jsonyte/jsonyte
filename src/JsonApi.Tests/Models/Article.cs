using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class Article
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
