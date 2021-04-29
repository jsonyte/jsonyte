namespace Jsonyte.Tests.Models
{
    public class ModelWithReadOnlyFields
    {
        public string Id = "1";

        public string Type = "model";

        public readonly int? IntValue = 5;

        public readonly string StringValue = "str";

        public readonly Author Author = new()
        {
            Id = "4",
            Type = "author",
            Name = "bob",
            Twitter = "bo"
        };
    }
}
