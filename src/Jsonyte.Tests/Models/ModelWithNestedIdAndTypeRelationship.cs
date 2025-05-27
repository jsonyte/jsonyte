using Jsonyte.Serialization;
using Jsonyte.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-with-nested-id-and-type-relationship")]
    internal class ModelWithNestedIdAndTypeRelationship
    {
        public string Id { get; set; }

        public ModelWithIdAndTypeRelationship Model { get; set; }
    }
}
