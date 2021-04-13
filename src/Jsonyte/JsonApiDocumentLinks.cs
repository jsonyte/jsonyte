using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiDocumentLinks : JsonApiLinks
    {
        private const string FirstKey = "first";

        private const string LastKey = "last";

        private const string PrevKey = "prev";

        private const string NextKey = "next";

        private const string SelfKey = "self";

        private const string RelatedKey = "related";

        [JsonPropertyName(FirstKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? First
        {
            get => GetOrNull(FirstKey);
            set => SetOrRemove(FirstKey, value);
        }

        [JsonPropertyName(LastKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Last
        {
            get => GetOrNull(LastKey);
            set => SetOrRemove(LastKey, value);
        }

        [JsonPropertyName(PrevKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Prev
        {
            get => GetOrNull(PrevKey);
            set => SetOrRemove(PrevKey, value);
        }

        [JsonPropertyName(NextKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Next
        {
            get => GetOrNull(NextKey);
            set => SetOrRemove(NextKey, value);
        }

        [JsonPropertyName(SelfKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Self
        {
            get => GetOrNull(SelfKey);
            set => SetOrRemove(SelfKey, value);
        }

        [JsonPropertyName(RelatedKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Related
        {
            get => GetOrNull(RelatedKey);
            set => SetOrRemove(RelatedKey, value);
        }
    }
}
