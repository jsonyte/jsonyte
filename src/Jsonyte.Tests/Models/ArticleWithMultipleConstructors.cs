using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithMultipleConstructors
    {
        [JsonConstructor]
        private ArticleWithMultipleConstructors(string id)
        {
        }

        [JsonConstructor]
        public ArticleWithMultipleConstructors(string id, string type, string title)
        {
            Id = id;
            Type = type;
            Title = title;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
