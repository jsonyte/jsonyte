using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents a <see href="https://jsonapi.org/">JSON:API</see> resource in a document.
    /// </summary>
    /// <typeparam name="T">The type of the resource data.</typeparam>
    public sealed class JsonApiResource<T>
    {
        /// <summary>
        /// Gets the id that identifies the resource.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets the attributes of the resource's data.
        /// </summary>
        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Attributes { get; set; }

        /// <summary>
        /// Gets an object representing the relationships between the resource and other JSON:API resource.
        /// </summary>
        [JsonPropertyName("relationships")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiRelationship>? Relationships { get; set; }

        /// <summary>
        /// Gets the links that relate to the resource.
        /// </summary>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResourceLinks? Links { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the resource.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }

    /// <summary>
    /// Represents a <see href="https://jsonapi.org/">JSON:API</see> resource in a document.
    /// </summary>
    public sealed class JsonApiResource
    {
        /// <summary>
        /// Gets the id that identifies the resource.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets the attributes of the resource's data.
        /// </summary>
        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonElement>? Attributes { get; set; }

        /// <summary>
        /// Gets an object representing the relationships between the resource and other JSON:API resource.
        /// </summary>
        [JsonPropertyName("relationships")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, JsonApiRelationship>? Relationships { get; set; }

        /// <summary>
        /// Gets the links that relate to the resource.
        /// </summary>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResourceLinks? Links { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the resource.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
