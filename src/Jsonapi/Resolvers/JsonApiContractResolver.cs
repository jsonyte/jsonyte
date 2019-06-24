using System;
using Jsonapi.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Resolvers
{
    public class JsonApiContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(JToken).IsAssignableFrom(objectType))
            {
                return base.ResolveContractConverter(objectType);
            }

            if (objectType.IsDocument())
            {
                return new DocumentConverter();
            }

            if (objectType.IsResource())
            {
                return new ResourceConverter();
            }

            if (objectType.IsRelationship())
            {
                return new RelationshipConverter();
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}
