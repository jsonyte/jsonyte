namespace Jsonyte.Tests.Models
{
    public class ModelReferencingItself
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public ModelReferencingItself[] Itself { get; set; }
    }
}
