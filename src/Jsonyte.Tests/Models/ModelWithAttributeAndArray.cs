using System;
using Jsonyte.Serialization.Attributes;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-attribute-array")]
    public class ModelWithAttributeAndArray
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public ModelWithAttribute[] AssociatedObjects { get; set; }
    }
}
