using System;
using Jsonyte.Serialization.Attributes;

namespace Jsonyte.Tests.Models
{
    [ResourceObject("model-with-attribute")]
    public class ModelWithAttribute
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public int IntValue { get; set; }
    }
}