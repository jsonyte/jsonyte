using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static object ResolveDocument(this JsonSerializer serializer)
        {
            return serializer.ReferenceResolver.ResolveReference(null, JsonApiMembers.Document);
        }

        public static Queue<object> ResolveIncludedResources(this JsonSerializer serializer)
        {
            return serializer.ReferenceResolver.ResolveReference(null, JsonApiMembers.Included) as Queue<object>;
        }

        public static JsonObjectContract ResolveObjectContract(this JsonSerializer serializer, Type type)
        {
            return serializer.ContractResolver.ResolveContract(type) as JsonObjectContract;
        }

        public static void Populate(this JsonSerializer serializer, JsonReader reader, object target, string member)
        {
            var contract = serializer.ResolveObjectContract(target.GetType());

            var property = contract.Properties.GetClosestMatchProperty(member);

            if (property != null && property.Writable && !property.Ignored)
            {
                var value = serializer.Deserialize(reader, property.PropertyType);

                property.ValueProvider.SetValue(target, value);
            }
        }
    }
}
