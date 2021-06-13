using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents the links that relate to the primary data of a <see href="https://jsonapi.org/">JSON:API</see> document.
    /// </summary>
    public class JsonApiDocumentLinks : JsonApiLinks
    {
        private const string FirstKey = "first";

        private const string LastKey = "last";

        private const string PrevKey = "prev";

        private const string NextKey = "next";

        private const string SelfKey = "self";

        private const string RelatedKey = "related";

        /// <summary>
        /// Gets a link to the first page of data when using pagination.
        /// </summary>
        [JsonPropertyName(FirstKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? First
        {
            get => GetOrNull(FirstKey);
            set => SetOrRemove(FirstKey, value);
        }

        /// <summary>
        /// Gets a link to the last page of data when using pagination.
        /// </summary>
        [JsonPropertyName(LastKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Last
        {
            get => GetOrNull(LastKey);
            set => SetOrRemove(LastKey, value);
        }

        /// <summary>
        /// Gets a link to the previous page of data when using pagination.
        /// </summary>
        [JsonPropertyName(PrevKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Prev
        {
            get => GetOrNull(PrevKey);
            set => SetOrRemove(PrevKey, value);
        }

        /// <summary>
        /// Gets a link to the next page of data when using pagination.
        /// </summary>
        [JsonPropertyName(NextKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Next
        {
            get => GetOrNull(NextKey);
            set => SetOrRemove(NextKey, value);
        }

        /// <summary>
        /// Gets a link that generated the current document.
        /// </summary>
        [JsonPropertyName(SelfKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Self
        {
            get => GetOrNull(SelfKey);
            set => SetOrRemove(SelfKey, value);
        }

        /// <summary>
        /// Gets a link to a related resource when the primary data represents a resource relationship.
        /// </summary>
        [JsonPropertyName(RelatedKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Related
        {
            get => GetOrNull(RelatedKey);
            set => SetOrRemove(RelatedKey, value);
        }
    }
}
