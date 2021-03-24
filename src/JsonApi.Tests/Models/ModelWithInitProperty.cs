using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
#if NET5_0_OR_GREATER
    public class ModelWithInitProperty
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "model";

        [JsonPropertyName("title")]
        public string InitTitle { get; init; } = "value";
    }
#endif
}
