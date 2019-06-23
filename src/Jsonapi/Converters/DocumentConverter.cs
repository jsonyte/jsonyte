using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jsonapi.Converters
{
    public class DocumentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = (JsonObjectContract) serializer.ContractResolver.ResolveContract(objectType);
            var document = contract.DefaultCreator();

            serializer.ReferenceResolver.AddReference(null, JsonApiMembers.Document, document);

            var flags = DocumentFlags.None;

            foreach (var member in reader.GetMembers())
            {
                flags = AddFlags(flags, member);

                if (member == JsonApiMembers.Data)
                {
                    var dataType = objectType.GenericTypeArguments.First();

                    var value = serializer.Deserialize(reader, dataType);
                    var property = contract.Properties.GetClosestMatchProperty(member);

                    property.ValueProvider.SetValue(document, value);
                }
            }

            return document;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private DocumentFlags AddFlags(DocumentFlags flags, string member)
        {
            switch (member)
            {
                case JsonApiMembers.Data:
                    return flags | DocumentFlags.Data;

                case JsonApiMembers.Errors:
                    return flags | DocumentFlags.Errors;

                case JsonApiMembers.Meta:
                    return flags | DocumentFlags.Meta;
            }

            return flags;
        }
    }
}
