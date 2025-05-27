using Jsonyte.Serialization;
using Jsonyte.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-with-explicit-array-id-and-type-relationship")]
    internal class ModelWithExplicitArrayIdAndTypeRelationship
    {
        public string Id { get; set; }

        [SerializeAs(type: RelationshipSerializationType.IdAndType)]
        public JsonApiRelationship<ModelWithAttribute[]> ExplicitArray { get; set; }
    }
}
