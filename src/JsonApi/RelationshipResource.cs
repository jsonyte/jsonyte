namespace JsonApi
{
    internal struct RelationshipResource<T>
    {
        public readonly T Resource;

        public RelationshipResource(T resource)
        {
            Resource = resource;
        }
    }
}
