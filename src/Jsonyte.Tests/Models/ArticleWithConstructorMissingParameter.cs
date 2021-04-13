using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithConstructorMissingParameter
    {
        public ArticleWithConstructorMissingParameter(string id, string type)
        {
            Id = id;
            Type = type;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
