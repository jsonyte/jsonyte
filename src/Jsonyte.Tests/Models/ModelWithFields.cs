using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithFields
    {
        [JsonPropertyName("type")]
        public string Type = "type";

        [JsonPropertyName("readOnlyTitle")]
        private readonly string readOnlyTitle = "title";

        [JsonPropertyName("readWriteTitle")]
        private string readWriteTitle = "title";

        [JsonPropertyName("publicTitle")]
        public string PublicTitle = "title";

        [JsonPropertyName("publicReadOnlyTitle")]
        public readonly string PublicReadOnlyTitle = "title";

        [JsonPropertyName("readWriteCount")]
        private int readWriteCount = 5;

        [JsonPropertyName("publicCount")]
        public int PublicCount = 5;

        [JsonPropertyName("readWriteNullableCount")]
        private int? readWriteNullableCount = 5;

        [JsonPropertyName("publicNullableCount")]
        public int? PublicNullableCount = 5;

        [JsonPropertyName("publicReadOnlyCount")]
        public readonly int PublicReadOnlyCount = 5;

        [JsonPropertyName("publicReadOnlyNullableCount")]
        public readonly int? PublicReadOnlyNullableCount = 5;
    }
}
