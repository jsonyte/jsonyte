using System;
using System.Linq;
using System.Text.Json;
using JsonApi.Serialization;
using JsonApi.Validation;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiResourceObjectConverter<T> : WrappedJsonConverter<T>
    {
        private readonly JsonTypeInfo info;

        public JsonApiResourceObjectConverter(JsonTypeInfo info)
        {
            this.info = info;
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resource = default(T);

            var documentState = reader.ReadDocument();
            var tracked = new TrackedResources();

            Utf8JsonReader savedReader = default;
            var includedReadFirst = false;

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref documentState);

                if (name == JsonApiMembers.Data)
                {
                    resource = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else if (name == JsonApiMembers.Included)
                {
                    if (documentState.HasFlag(DocumentFlags.Data))
                    {
                        ReadIncluded(ref reader, ref tracked, options);
                    }
                    else
                    {
                        includedReadFirst = true;
                        savedReader = reader;

                        reader.Skip();
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            // TODO
            // Really janky way of doing this as it means parsing over included twice in some instances
            // This needs to be re-thought
            if (includedReadFirst)
            {
                ReadIncluded(ref savedReader, ref tracked, options);
            }

            tracked.Release();

            documentState.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var resourceState = reader.ReadResource();

            var resource = existingValue ?? info.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref resourceState);

                var property = info.GetMember(name);

                if (name == JsonApiMembers.Id || name == JsonApiMembers.Type)
                {
                    property.Read(ref reader, resource);
                }
                else if (name == JsonApiMembers.Attributes)
                {
                    reader.ReadObject(JsonApiMemberCode.ResourceAttributes);

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember(JsonApiMemberCode.Resource);

                        info.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else if (name == JsonApiMembers.Meta)
                {
                    property.Read(ref reader, resource);
                }
                else if (name == JsonApiMembers.Relationships)
                {
                    ReadRelationships(ref reader, ref tracked, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            resourceState.Validate();

            return (T) resource;
        }

        private void ReadRelationships(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            reader.ReadObject(JsonApiMemberCode.Relationship);

            while (reader.IsInObject())
            {
                var relationshipName = reader.ReadMember(JsonApiMemberCode.Relationship);

                info.GetMember(relationshipName).ReadRelationship(ref reader, ref tracked, resource);

                reader.Read();
            }
        }

        private void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options)
        {
            reader.ReadArray(JsonApiArrayCode.Included);

            while (reader.IsInArray())
            {
                var identifier = reader.ReadAheadIdentifier();

                if (tracked.TryGetIncluded(identifier, out var included))
                {
                    included.Converter.Read(ref reader, ref tracked, included.Value, options);
                }
                else
                {
                    throw new JsonApiFormatException("JSON:API included resource must be referenced by at least one relationship");
                }

                reader.Read();
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            writer.WriteStartObject();

            writer.WritePropertyName(JsonApiMembers.DataEncoded);
            WriteWrapped(writer, ref tracked, value, options);
            
            if (tracked.Count > 0)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                writer.WriteStartArray();

                while (tracked.Identifiers.Count > 0)
                {
                    var identifier = tracked.Identifiers.Dequeue();

                    if (tracked.TryGetIncluded(identifier, out var included))
                    {
                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }
                }

                foreach (var identifier in tracked.Identifiers)
                {
                    if (tracked.TryGetIncluded(identifier, out var included))
                    {
                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();

                return;
            }

            writer.WriteStartObject();

            info.IdMember.Write(writer, ref tracked, value);
            info.TypeMember.Write(writer, ref tracked, value);

            if (info.AttributeMembers.Any())
            {
                writer.WritePropertyName(JsonApiMembers.AttributesEncoded);
                writer.WriteStartObject();

                foreach (var member in info.AttributeMembers)
                {
                    member.Write(writer, ref tracked, value);
                }

                writer.WriteEndObject();
            }

            if (info.RelationshipMembers.Any())
            {
                var relationshipsWritten = false;

                foreach (var member in info.RelationshipMembers)
                {
                    member.WriteRelationship(writer, ref tracked, value, ref relationshipsWritten);
                }

                if (relationshipsWritten)
                {
                    writer.WriteEndObject();
                }
            }

            info.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }
    }
}
