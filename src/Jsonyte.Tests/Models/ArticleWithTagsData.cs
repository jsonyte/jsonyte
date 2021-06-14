using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithTagsData
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public List<TagWithData> Tags { get; set; } = new();
    }
}
