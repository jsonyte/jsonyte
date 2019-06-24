using Newtonsoft.Json.Linq;

namespace Jsonapi.Extensions
{
    public static class JObjectExtensions
    {
        public static ResourceIdentifier GetResourceIdentifier(this JObject value)
        {
            var id = value[JsonApiMembers.Id].Value<string>();
            var type = value[JsonApiMembers.Type].Value<string>();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                throw new JsonApiException("Resource identifier must contain valid Id and Type");
            }

            return new ResourceIdentifier(id, type);
        }
    }
}
