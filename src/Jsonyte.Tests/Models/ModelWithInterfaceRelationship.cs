namespace Jsonyte.Tests.Models
{
    public class ModelWithInterfaceRelationship
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public IArticle Article { get; set; }
    }
}
