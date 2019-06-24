using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Extensions
{
    public static class JsonSerializerExtensions
    {
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

        public static void AddReference(this JsonSerializer serializer, object target)
        {
            var contract = serializer.ResolveObjectContract(target.GetType());

            var idProperty = contract.Properties.GetClosestMatchProperty(JsonApiMembers.Id);
            var typeProperty = contract.Properties.GetClosestMatchProperty(JsonApiMembers.Type);

            if (idProperty != null && typeProperty != null)
            {
                var id = idProperty.ValueProvider.GetValue(target);
                var type = typeProperty.ValueProvider.GetValue(target);

                var key = $"{type}:{id}";

                serializer.ReferenceResolver.AddReference(null, key, target);
            }
        }
    }
}
