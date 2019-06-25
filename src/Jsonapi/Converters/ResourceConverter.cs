using System;
using System.Linq;
using Jsonapi.Extensions;
using Newtonsoft.Json;

namespace Jsonapi.Converters
{
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializer.ResolveDocument() == null)
            {
                var document = CreateContainer(typeof(JsonApiDocument<>), value, serializer);
                serializer.Serialize(writer, document);

                return;
            }

            var contract = serializer.ResolveObjectContract(value.GetType());

            writer.WriteStartObject();

            var identifiers = contract.Properties
                .Where(x => x.PropertyName == JsonApiMembers.Id || x.PropertyName == JsonApiMembers.Type)
                .OrderBy(x => x.PropertyName);

            var attributes = contract.Properties
                .Where(x => x.PropertyName != JsonApiMembers.Id && x.PropertyName != JsonApiMembers.Type)
                .Where(x => !x.PropertyType.IsResource())
                .ToArray();

            var relationships = contract.Properties
                .Where(x => x.PropertyType.IsResource())
                .ToArray();

            foreach (var property in identifiers)
            {
                writer.WritePropertyName(property.PropertyName);
                writer.WriteValue(property.ValueProvider.GetValue(value).ToString());
            }

            if (attributes.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Attributes);
                writer.WriteStartObject();

                foreach (var property in attributes)
                {
                    writer.WritePropertyName(property.PropertyName);
                    writer.WriteValue(property.ValueProvider.GetValue(value));
                }

                writer.WriteEndObject();
            }

            if (relationships.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Relationships);
                writer.WriteStartObject();

                foreach (var property in relationships)
                {
                    writer.WritePropertyName(property.PropertyName);
                    writer.WriteStartObject();

                    var data = property.ValueProvider.GetValue(value);
                    var relationship = CreateContainer(typeof(JsonApiRelationship<>), data, serializer);

                    serializer.Serialize(writer, relationship);

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (serializer.ResolveDocument() == null)
            {
                return ReadObject(reader, typeof(JsonApiDocument<>), objectType, serializer);
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

                if (member == JsonApiMembers.Relationships)
                {
                    foreach (var relationshipMember in reader.GetMembers())
                    {
                        var property = contract.Properties.GetClosestMatchProperty(relationshipMember);

                        var value = ReadObject(reader, typeof(JsonApiRelationship<>), property.PropertyType, serializer);

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

        private object CreateContainer(Type containerType, object data, JsonSerializer serializer)
        {
            var type = containerType.MakeGenericType(data.GetType());
            var contract = serializer.ResolveObjectContract(type);

            var document = contract.DefaultCreator();

            contract.Properties.GetClosestMatchProperty(JsonApiMembers.Data)
                .ValueProvider
                .SetValue(document, data);

            return document;
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
