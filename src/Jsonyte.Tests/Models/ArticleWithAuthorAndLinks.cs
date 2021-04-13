using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithAuthorAndLinks
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public JsonApiRelationship<Author> Author { get; set; }

        [JsonPropertyName("comments")]
        public JsonApiRelationship<Comment[]> Comments { get; set; }
    }
}
