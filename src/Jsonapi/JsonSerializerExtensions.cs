using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi
{
    public static class JsonSerializerExtensions
    {
        public static void Populate(this JsonSerializer serializer, JsonReader reader, object target, string member)
        {
            var contract = (JsonObjectContract) serializer.ContractResolver.ResolveContract(target.GetType());

            var property = contract.Properties.GetClosestMatchProperty(member);

            if (property != null && property.Writable && !property.Ignored)
            {
                var value = serializer.Deserialize(reader, property.PropertyType);

                property.ValueProvider.SetValue(target, value);
            }
        }
    }
}
