namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct InlineResource<T>
    {
        public readonly T? Resource;

        public InlineResource(T? resource)
        {
            Resource = resource;
        }
    }
}
