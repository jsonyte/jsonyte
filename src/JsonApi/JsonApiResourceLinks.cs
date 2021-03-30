﻿using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiResourceLinks : JsonApiLinks
    {
        private const string SelfKey = "self";

        [JsonPropertyName(SelfKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Self
        {
            get => GetOrNull(SelfKey);
            set => SetOrRemove(SelfKey, value);
        }
    }
}
