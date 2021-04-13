using System.Text.Json.Serialization;
using JsonApi.Tests.Converters;

namespace JsonApi.Tests.Models
{
    public class ArticleWithIsbnAndConverter
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("isbn")]
        [JsonConverter(typeof(IsbnConverter))]
        public Isbn Isbn { get; set;}
    }
}
