using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithNullableTypes
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }
    }
}
