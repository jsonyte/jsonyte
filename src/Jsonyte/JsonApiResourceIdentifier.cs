using System;
using System.Text.Json.Serialization;

namespace Jsonyte
{
    public struct JsonApiResourceIdentifier : IEquatable<JsonApiResourceIdentifier>
    {
        [JsonConstructor]
        public JsonApiResourceIdentifier(string id, string type)
        {
            Id = id;
            Type = type;
            Meta = null;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        public bool Equals(JsonApiResourceIdentifier other)
        {
            return Id == other.Id && Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            return obj is JsonApiResourceIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();

                return hash;
            }
        }
    }
}
