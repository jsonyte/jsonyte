namespace JsonApi
{
    internal struct RelationshipResource<T>
    {
        public T Resource;

        public RelationshipResource(T resource)
        {
            Resource = resource;
        }
    }
}
