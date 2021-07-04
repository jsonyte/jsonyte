namespace Jsonyte.Tests.Models
{
    public class HoldingBank
    {
        public string Id { get; set; }

        public string Type => "banks";

        public string Name { get; set; }

        public Tag Tag { get; set; }
    }
}
