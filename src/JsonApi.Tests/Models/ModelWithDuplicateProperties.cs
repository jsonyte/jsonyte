﻿using System.Text.Json.Serialization;

namespace JsonApi.Tests.Models
{
    public class ModelWithDuplicateProperties
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("name")]
        public string AlsoName { get; set; }
    }
}
