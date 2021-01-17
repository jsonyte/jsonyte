using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationshipLinks
    {
        [JsonPropertyName("self")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink Self { get; set; }

        [JsonPropertyName("related")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink Related { get; set; }
    }
}
