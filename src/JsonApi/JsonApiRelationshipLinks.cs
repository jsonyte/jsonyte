using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationshipLinks
    {
        [JsonPropertyName("self")]
        public JsonApiLink Self { get; set; }

        [JsonPropertyName("related")]
        public JsonApiLink Related { get; set; }
    }
}
