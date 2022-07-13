using System;
using Jsonyte.Serialization.Attributes;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-with-attribute-and-type")]
    public class ModelWithAttributeAndType
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public int IntValue { get; set; }
    }
}

