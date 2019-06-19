using Jsonapi.Resolvers;
using Newtonsoft.Json;

namespace Jsonapi
{
    public class JsonApiSerializerSettings : JsonSerializerSettings
    {
        public JsonApiSerializerSettings()
        {
            ContractResolver = new JsonApiContractResolver();
            ReferenceResolverProvider = () => new JsonApiReferenceResolver();
        }
    }
}
