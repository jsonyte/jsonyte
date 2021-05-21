namespace Jsonyte.Tests.Models
{
    public class ModelWithCircularType
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ModelWithAnotherCircularType First { get; set; }
    }
}
