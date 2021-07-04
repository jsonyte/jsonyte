using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class AccountWithAttributes
    {
        public string Id { get; set; }

        public string Type => "accounts";

        [JsonPropertyName("myBank")]
        public HoldingBank Bank { get; set; }
    }
}
