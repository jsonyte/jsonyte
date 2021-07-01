using Xunit;

namespace Jsonyte.Tests.Queries
{
    public class Article
    {
        public string Id { get; set; }

        public string Type { get; } = "articles";

        public Author Author { get; set; }

        public Tag[] Tags { get; set; }
    }

    public class Author
    {
        public string Id { get; set; }

        public string Type { get; } = "authors";

        public Tag[] Tags { get; set; }
    }

    public class Tag
    {
        public string Id { get; set; }

        public string Type { get; } = "tags";

        public string Name { get; set; }
    }

    public class JsonApiQueryBuilderTests
    {
        [Fact]
        public void Test()
        {
            var builder = new JsonApiQueryBuilder<Article>()
                .IncludeField(x => x.Id);
        }
    }
}
