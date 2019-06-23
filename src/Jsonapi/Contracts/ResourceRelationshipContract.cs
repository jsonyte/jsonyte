using System;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Contracts
{
    public class ResourceRelationshipContract : JsonObjectContract
    {
        public ResourceRelationshipContract(Type underlyingType)
            : base(underlyingType)
        {
        }
    }
}
