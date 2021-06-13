using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents a relationship to another resource for a <see href="https://jsonapi.org/">JSON:API</see> document.
    /// </summary>
    /// <remarks>
    /// The relationship can be either one-to-one or one-to-many, and can be an external
    /// relationship or a relationship to a resource in the same JSON:API document.
    /// </remarks>
    public sealed class JsonApiRelationship
    {
        /// <summary>
        /// Gets the resource linkage to the related data.
        /// </summary>
        /// <remarks>
        /// Resource linkage is represented by using the 'id' and 'type' members only.
        /// </remarks>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResourceIdentifier[]? Data { get; set; }

        /// <summary>
        /// Gets the links that relate to the resource relationship.
        /// </summary>
        /// <remarks>
        /// Resource links must contain at least a 'self' or a 'related' property.
        /// </remarks>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiRelationshipLinks? Links { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the resource relationship.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }

    /// <summary>
    /// Represents a relationship to another resource.
    /// </summary>
    /// <remarks>
    /// The relationship can be either one-to-one or one-to-many, and can be an external
    /// relationship or a relationship to a resource in the same JSON:API document.
    /// </remarks>
    public sealed class JsonApiRelationship<T>
    {
        /// <summary>
        /// Gets the related data or resource linkage to the related data.
        /// </summary>
        /// <remarks>
        /// Resource linkage is represented by using the 'id' and 'type' members only. In the case of relationships
        /// within the same JSON:API document, the relationship data is included in the document and object graph.
        /// </remarks>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        /// <summary>
        /// Gets the links that relate to the resource relationship.
        /// </summary>
        /// <remarks>
        /// Resource links must contain at least a 'self' or a 'related' property.
        /// </remarks>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiRelationshipLinks? Links { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the resource relationship.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
