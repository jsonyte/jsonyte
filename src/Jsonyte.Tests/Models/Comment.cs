using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class Comment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set;}

        [JsonPropertyName("author")]
        public Author Author { get; set; }
    }
}
