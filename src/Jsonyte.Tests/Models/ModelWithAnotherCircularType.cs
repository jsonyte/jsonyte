namespace Jsonyte.Tests.Models
{
    public class ModelWithAnotherCircularType
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ModelWithCircularType Second { get; set; }
    }
}
