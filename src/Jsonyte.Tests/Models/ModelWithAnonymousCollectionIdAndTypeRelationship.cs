using Jsonyte.Serialization;
using Jsonyte.Serialization.Attributes;
using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    [JsonApiResource("model-with-anonymous-collection-id-and-type-relationship")]
    internal class ModelWithAnonymousCollectionIdAndTypeRelationship
    {
        public string Id { get; set; }

        [SerializeAs(type: RelationshipSerializationType.IdAndType)]
        public IEnumerable<ModelWithAttribute> AnonymousCollection { get; set; }
    }
}
