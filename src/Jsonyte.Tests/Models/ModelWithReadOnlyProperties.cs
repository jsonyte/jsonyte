namespace Jsonyte.Tests.Models
{
    public class ModelWithReadOnlyProperties
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public int? IntValue { get; } = 5;

        public Author Author { get; } = new()
        {
            Id = "4",
            Type = "author",
            Name = "Bob"
        };
    }
}
