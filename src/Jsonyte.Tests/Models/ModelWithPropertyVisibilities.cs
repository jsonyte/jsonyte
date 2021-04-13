using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithPropertyVisibilities
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "model";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "value";

        [JsonPropertyName("readOnlyTitle")]
        public string ReadOnlyTitle { get; } = "value";

        [JsonPropertyName("writeOnlyTitle")]
        public string WriteOnlyTitle { private get; set; } = "value";

        [JsonPropertyName("count")]
        public int Count { get; set; } = 5;

        [JsonPropertyName("nullableCount")]
        public int? NullableCount { get; set; } = 5;

        [JsonPropertyName("readOnlyCount")]
        public int ReadOnlyCount { get; } = 5;

        [JsonPropertyName("writeOnlyCount")]
        public int WriteOnlyCount { private get; set; } = 5;
    }
}
