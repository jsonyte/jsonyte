using Jsonyte.Serialization;
using Jsonyte.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-referencing-itself-with-id-and-type-relationship")]
    internal class ModelReferencingItselfWithIdAndTypeRelationship
    {
        public string Id { get; set; }

        public string Value { get; set; }

        [SerializeAs(type: RelationshipSerializationType.IdAndType)]
        public ModelReferencingItselfWithIdAndTypeRelationship Itself { get; set; }
    }
}
