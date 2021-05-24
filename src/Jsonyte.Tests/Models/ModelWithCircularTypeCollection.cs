namespace Jsonyte.Tests.Models
{
    public class ModelWithCircularTypeCollection
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ModelWithCircularTypeCollectionNested[] First { get; set; }
    }
}
