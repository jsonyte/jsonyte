namespace Jsonyte.Tests.Models
{
    public class ArticleWithNestedArticles
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public ArticleWithNestedArticles Referenced { get; set; }
    }
}
