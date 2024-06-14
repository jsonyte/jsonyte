namespace Jsonyte.Tests.Models
{
    public class Transaction
    {
        public string Id { get; set; }

        public string Type => "transactions";

        public string FromAccount { get; set; }

        public string ToAccount { get; set; }

        public decimal Amount { get; set; }

        public Tag Tag { get; set; }
    }
}
