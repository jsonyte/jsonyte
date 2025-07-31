using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class Account
    {
        public string Id { get; set; }

        public string Type => "accounts";

        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public HoldingBank HoldingBank { get; set; }

        public Transaction[] Transactions { get; set; }

        [JsonPropertyName("myBank")]
        public HoldingBank OwnerBank { get; set; }
    }
}
