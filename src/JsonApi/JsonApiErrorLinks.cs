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
        public JsonApiLink About
        {
            get => GetOrNull("about");
            set => base["about"] = value;
        }

        private JsonApiLink GetOrNull(string key)
        {
            return TryGetValue(key, out var value)
                ? value
                : null;
        }
    }
}
