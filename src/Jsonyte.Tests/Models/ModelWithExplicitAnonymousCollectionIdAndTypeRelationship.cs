using Jsonyte.Serialization;
using Jsonyte.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-with-explicit-anonymous-collection-id-and-type-relationship")]
    internal class ModelWithExplicitAnonymousCollectionIdAndTypeRelationship
    {
        public string Id { get; set; }

        [SerializeAs(type: RelationshipSerializationType.IdAndType)]
        public JsonApiRelationship<IEnumerable<ModelWithAttribute>> ExplicitAnonymousCollection { get; set; }
    }
}
