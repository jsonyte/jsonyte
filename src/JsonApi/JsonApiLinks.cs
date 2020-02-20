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
        public JsonApiLink First
        {
            get => GetOrNull("first");
            set => base["first"] = value;
        }

        [JsonPropertyName("last")]
        public JsonApiLink Last
        {
            get => GetOrNull("last");
            set => base["last"] = value;
        }

        [JsonPropertyName("prev")]
        public JsonApiLink Prev
        {
            get => GetOrNull("prev");
            set => base["prev"] = value;
        }

        [JsonPropertyName("next")]
        public JsonApiLink Next
        {
            get => GetOrNull("next");
            set => base["next"] = value;
        }

        [JsonPropertyName("self")]
        public JsonApiLink Self
        {
            get => GetOrNull("self");
            set => base["self"] = value;
        }

        [JsonPropertyName("related")]
        public JsonApiLink Related
        {
            get => GetOrNull("self");
            set => base["self"] = value;
        }

        private JsonApiLink GetOrNull(string key)
        {
            return TryGetValue(key, out var value)
                ? value
                : null;
        }
    }
}
