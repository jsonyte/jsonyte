using System;
using System.Linq;
using Jsonapi.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jsonapi.Converters
{
    public class RelationshipConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contract = serializer.ResolveObjectContract(value.GetType());
            var dataContract = serializer.ResolveObjectContract(value.GetType().GenericTypeArguments.First());

            var resources = serializer.ResolveIncludedResources();

            var data = contract.Properties
                .GetClosestMatchProperty(JsonApiMembers.Data)
                .ValueProvider
                .GetValue(value);

            writer.WritePropertyName(JsonApiMembers.Data);
            writer.WriteStartObject();

            var identifiers = dataContract.Properties
                .Where(x => x.PropertyName == JsonApiMembers.Id || x.PropertyName == JsonApiMembers.Type)
                .OrderBy(x => x.PropertyName);

            foreach (var property in identifiers)
            {
                writer.WritePropertyName(property.PropertyName);
                writer.WriteValue(property.ValueProvider.GetValue(data).ToString());
            }

            resources.Enqueue(data);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = serializer.ResolveObjectContract(objectType);
            var relationship = existingValue ?? contract.DefaultCreator();

            var flags = RelationshipFlags.None;

            foreach (var member in reader.GetMembers())
            {
                flags = AddFlags(flags, member);

                if (member == JsonApiMembers.Data)
                {
                    var dataType = objectType.GenericTypeArguments.First();
                    var dataContract = serializer.ResolveObjectContract(dataType);

                    var dataJson = serializer.Deserialize<JObject>(reader);

                    var key = ResourceIdentifier.Create(dataJson);
                    
                    var existingData = serializer.ReferenceResolver.ResolveReference(null, key.ToString());
                    var data = dataContract.DefaultCreator();

                    if (existingData == null)
                    {
                        dataContract.Converter.ReadJson(dataJson.CreateReader(), dataType, data, serializer);

                        serializer.ReferenceResolver.AddReference(null, key.ToString(), data);
                    }
                    else if (existingData is JObject existingDataJson)
                    {
                        dataContract.Converter.ReadJson(existingDataJson.CreateReader(), dataType, data, serializer);
                    }

                    contract.Properties
                        .GetClosestMatchProperty(JsonApiMembers.Data)
                        .ValueProvider
                        .SetValue(relationship, data);
                }
            }

            return relationship;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsRelationship();
        }

        private RelationshipFlags AddFlags(RelationshipFlags flags, string member)
        {
            switch (member)
            {
                case JsonApiMembers.Links:
                    return flags | RelationshipFlags.Links;

                case JsonApiMembers.Data:
                    return flags | RelationshipFlags.Data;

                case JsonApiMembers.Meta:
                    return flags | RelationshipFlags.Meta;
            }

            return flags;
        }
    }
}
