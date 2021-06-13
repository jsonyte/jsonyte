namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct RelationshipResource<T>
    {
        public readonly T Resource;

        public RelationshipResource(T resource)
        {
            Resource = resource;
        }
    }
}
