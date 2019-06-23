using System;
using Jsonapi.Converters;
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
            if (objectType.IsDocument())
            {
                return new DocumentConverter();
            }

            if (objectType.IsResource())
            {
                return new ResourceConverter();
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}
