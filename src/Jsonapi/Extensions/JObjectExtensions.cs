using Newtonsoft.Json.Linq;

namespace Jsonapi.Extensions
{
    public static class JObjectExtensions
    {
        public static ResourceIdentifier GetResourceIdentifier(this JObject value)
        {
            var id = value[JsonApiMembers.Id].Value<string>();
            var type = value[JsonApiMembers.Type].Value<string>();

            return new ResourceIdentifier(id, type);
        }
    }
}
