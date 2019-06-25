using System;
using System.Collections.Generic;
using System.Linq;
using Jsonapi.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jsonapi.Converters
{
    public class DocumentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contract = serializer.ResolveObjectContract(value.GetType());

            var resources = new Queue<object>();

            serializer.ReferenceResolver.AddReference(null, JsonApiMembers.Document, value);
            serializer.ReferenceResolver.AddReference(null, JsonApiMembers.Included, resources);

            var data = contract.Properties
                .GetClosestMatchProperty(JsonApiMembers.Data)
                .ValueProvider
                .GetValue(value);

            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.Data);

            serializer.ContractResolver
                .ResolveContract(data.GetType())
                .Converter
                .WriteJson(writer, data, serializer);

            if (resources.Any())
            {
                writer.WritePropertyName(JsonApiMembers.Included);
                writer.WriteStartArray();

                while (resources.Any())
                {
                    var resource = resources.Dequeue();

                    serializer.Serialize(writer, resource);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = serializer.ResolveObjectContract(objectType);
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

                    contract.Properties.GetClosestMatchProperty(member)
                        .ValueProvider
                        .SetValue(document, value);
                }

                if (member == JsonApiMembers.Included)
                {
                    foreach (var _ in reader.GetValues())
                    {
                        var resource = serializer.Deserialize<JObject>(reader);

                        var key = ResourceIdentifier.Create(resource);

                        var existingResource = serializer.ReferenceResolver.ResolveReference(null, key.ToString());

                        if (existingResource != null)
                        {
                            var resourceType = existingResource.GetType();

                            serializer.ContractResolver
                                .ResolveContract(resourceType)
                                .Converter
                                .ReadJson(resource.CreateReader(), resourceType, existingResource, serializer);
                        }
                        else
                        {
                            serializer.ReferenceResolver.AddReference(null, key.ToString(), resource);
                        }
                    }
                }
            }

            return document;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsDocument();
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
