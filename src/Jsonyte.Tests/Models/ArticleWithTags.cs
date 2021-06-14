using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithTags
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public List<Tag> Tags { get; set; } = new();
    }
}
