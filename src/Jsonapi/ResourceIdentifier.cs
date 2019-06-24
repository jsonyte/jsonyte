namespace Jsonapi
{
    public class ResourceIdentifier
    {
        public ResourceIdentifier(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public string Type { get; }

        public override string ToString()
        {
            return $"{Type}:{Id}";
        }
    }
}
