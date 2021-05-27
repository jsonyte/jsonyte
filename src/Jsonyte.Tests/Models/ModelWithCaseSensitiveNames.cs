using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ModelWithCaseSensitiveNames
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        [JsonPropertyName("ValueWithNameAttribute")]
        public string ValueWithNameAttribute { get; set; }
    }
}
