using System;
using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents an object that identifiers an individual <see href="https://jsonapi.org/">JSON:API</see> resource.
    /// </summary>
    public struct JsonApiResourceIdentifier : IEquatable<JsonApiResourceIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiResourceIdentifier"/> struct by using the specified strings.
        /// </summary>
        /// <param name="id">The id that represents the resource.</param>
        /// <param name="type">The type that represents the resource.</param>
        [JsonConstructor]
        public JsonApiResourceIdentifier(string id, string type)
        {
            Id = id;
            Type = type;
            Meta = null;
        }

        /// <summary>
        /// Gets the id of the resource identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the type of the resource identifier.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about resource identifier.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="JsonApiResourceIdentifier"/> represent the same value.
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to this instance; otherwise <c>false</c>.</returns>
        public bool Equals(JsonApiResourceIdentifier other)
        {
            return Id == other.Id && Type == other.Type;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="JsonApiResourceIdentifier"/> represent the same value.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is equal to this instance; otherwise <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is JsonApiResourceIdentifier other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
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
