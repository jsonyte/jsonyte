using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithIntId
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
