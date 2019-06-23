using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Converters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (serializer.ReferenceResolver.ResolveReference(null, JsonApiMembers.Document) == null)
            {
                return ReadDocument(reader, objectType, serializer);
            }

            var contract = (JsonObjectContract) serializer.ContractResolver.ResolveContract(objectType);
            var resource = existingValue ?? contract.DefaultCreator();

            var flags = ResourceFlags.None;

            foreach (var member in reader.GetMembers())
            {
                flags = AddFlags(flags, member);

                serializer.Populate(reader, resource, member);

                if (member == JsonApiMembers.Attributes)
                {
                    foreach (var attributeMember in reader.GetMembers())
                    {
                        serializer.Populate(reader, resource, attributeMember);
                    }
                }

                if (member == JsonApiMembers.Relationship)
                {
                    foreach (var relationshipMember in reader.GetMembers())
                    {
                        serializer.Populate(reader, resource, relationshipMember);
                    }
                }
            }

            return resource;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private object ReadDocument(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            var type = typeof(JsonApiDocument<>).MakeGenericType(objectType);

            return serializer.Deserialize(reader, type);
        }

        private ResourceFlags AddFlags(ResourceFlags flags, string member)
        {
            switch (member)
            {
                case JsonApiMembers.Id:
                    return flags | ResourceFlags.Id;

                case JsonApiMembers.Type:
                    return flags | ResourceFlags.Type;
            }

            return flags;
        }
    }
}
