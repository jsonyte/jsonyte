namespace Jsonyte.Tests.Models
{
    public class ArticleWithInterface : IArticle
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }
    }
}
