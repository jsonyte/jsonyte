using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithNoType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
