namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct AnonymousResourceCollection
    {
        public readonly object? Value;

        public AnonymousResourceCollection(object? value)
        {
            Value = value;
        }
    }
}
