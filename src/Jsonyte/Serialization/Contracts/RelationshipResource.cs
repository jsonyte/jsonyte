namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct RelationshipResource<T>
    {
        public readonly T Resource;

        public readonly RelationshipSerializationType RelationshipSerializationType;

        public RelationshipResource(T resource, RelationshipSerializationType relationshipSerializationType = RelationshipSerializationType.Included)
        {
            Resource = resource;
            RelationshipSerializationType = relationshipSerializationType;
        }
    }
}
