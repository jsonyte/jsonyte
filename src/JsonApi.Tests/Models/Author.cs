using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class Author
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("twitter")]
        public string Twitter { get; set; }
    }
}
