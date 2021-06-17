namespace Jsonyte.Tests.Models
{
    public class ArticleWithExplicitRelationship
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public JsonApiRelationship<Author> Author { get; set; }
    }
}
