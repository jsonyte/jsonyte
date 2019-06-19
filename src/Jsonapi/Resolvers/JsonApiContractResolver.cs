using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Resolvers
{
    public class JsonApiContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            return base.CreateObjectContract(objectType);
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            return base.ResolveContractConverter(objectType);
        }
    }
}
