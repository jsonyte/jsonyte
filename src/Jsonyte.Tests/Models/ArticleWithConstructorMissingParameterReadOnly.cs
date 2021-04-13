using System.Text.Json.Serialization;

namespace Jsonyte.Tests.Models
{
    public class ArticleWithConstructorMissingParameterReadOnly
    {
        public ArticleWithConstructorMissingParameterReadOnly(string id, string type)
        {
            Id = id;
            Type = type;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("title")]
        public string Title { get; }
    }
}
