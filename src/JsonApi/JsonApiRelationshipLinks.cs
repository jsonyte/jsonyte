using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationshipLinks : Dictionary<string, JsonApiLink>
    {
        private const string SelfKey = "self";

        private const string RelatedKey = "related";

        public JsonApiRelationshipLinks()
            : base(StringComparer.OrdinalIgnoreCase)
        {
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

        private JsonApiLink? GetOrNull(string key)
        {
            return TryGetValue(key, out var value)
                ? value
                : null;
        }

        private void SetOrRemove(string key, JsonApiLink? link)
        {
            if (link != null)
            {
                base[key] = link;
            }
            else
            {
                Remove(key);
            }
        }
    }
}
