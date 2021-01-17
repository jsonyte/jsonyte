using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiResource<T> : JsonApiResourceIdentifier
    {
        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Attributes { get; set; }

        [JsonPropertyName("relationships")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiRelationship> Relationships { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiLink> Links { get; set; }
    }

    public class JsonApiResource : JsonApiResourceIdentifier
    {
        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonElement> Attributes { get; set; }

        [JsonPropertyName("relationships")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiRelationship> Relationships { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiLink> Links { get; set; }
    }
}
