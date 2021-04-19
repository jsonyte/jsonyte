namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct AnonymousResource
    {
        public readonly object? Value;

        public AnonymousResource(object? value)
        {
            Value = value;
        }
    }
}
