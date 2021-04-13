using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class ArticleWithConstructor
    {
        public ArticleWithConstructor(string id)
        {
        }

        [JsonConstructor]
        public ArticleWithConstructor(string id, string type, string title)
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
