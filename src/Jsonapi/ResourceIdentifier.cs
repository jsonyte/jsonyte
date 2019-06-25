using Newtonsoft.Json.Linq;

namespace Jsonapi
{
    public class ResourceIdentifier
    {
        public static ResourceIdentifier Create(JObject value)
        {
            var id = value[JsonApiMembers.Id].Value<string>();
            var type = value[JsonApiMembers.Type].Value<string>();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                throw new JsonApiException("Resource identifier must contain valid Id and Type");
            }

            return new ResourceIdentifier(id, type);
        }

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
