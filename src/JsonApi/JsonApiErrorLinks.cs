using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiErrorLinks : Dictionary<string, JsonApiLink>
    {
        public JsonApiErrorLinks()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        [JsonPropertyName("about")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? About
        {
            get => GetOrNull("about");
            set => SetOrRemove("about", value);
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
