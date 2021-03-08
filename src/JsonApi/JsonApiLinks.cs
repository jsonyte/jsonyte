using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiLinks : Dictionary<string, JsonApiLink>
    {
        public JsonApiLinks()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        [JsonPropertyName("first")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? First
        {
            get => GetOrNull("first");
            set => SetOrRemove("first", value);
        }

        [JsonPropertyName("last")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Last
        {
            get => GetOrNull("last");
            set => SetOrRemove("last", value);
        }

        [JsonPropertyName("prev")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Prev
        {
            get => GetOrNull("prev");
            set => SetOrRemove("pref", value);
        }

        [JsonPropertyName("next")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Next
        {
            get => GetOrNull("next");
            set => SetOrRemove("next", value);
        }

        [JsonPropertyName("self")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Self
        {
            get => GetOrNull("self");
            set => SetOrRemove("self", value);
        }

        [JsonPropertyName("related")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Related
        {
            get => GetOrNull("related");
            set => SetOrRemove("related", value);
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
