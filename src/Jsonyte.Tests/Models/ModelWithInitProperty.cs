using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
#if NET5_0_OR_GREATER
    public class ModelWithInitProperty
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "model";

        [JsonPropertyName("initTitle")]
        public string InitTitle { get; init; } = "value";
    }
#endif
}
