using System;
using Jsonapi.Extensions;
using Newtonsoft.Json;

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

            var contract = serializer.ResolveObjectContract(objectType);
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
                        var property = contract.Properties.GetClosestMatchProperty(relationshipMember);

                        var value = ReadRelationship(reader, property.PropertyType, serializer);

                        property.ValueProvider.SetValue(resource, value);
                    }
                }
            }

            return resource;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsResource();
        }

        private object ReadDocument(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            return ReadObject(reader, typeof(JsonApiDocument<>), objectType, serializer);
        }

        private object ReadRelationship(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            return ReadObject(reader, typeof(JsonApiRelationship<>), objectType, serializer);
        }

        private object ReadObject(JsonReader reader, Type containerType, Type objectType, JsonSerializer serializer)
        {
            var type = containerType.MakeGenericType(objectType);

            var value = serializer.Deserialize(reader, type);

            return serializer.ResolveObjectContract(type)
                .Properties
                .GetClosestMatchProperty(JsonApiMembers.Data)
                .ValueProvider
                .GetValue(value);
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
