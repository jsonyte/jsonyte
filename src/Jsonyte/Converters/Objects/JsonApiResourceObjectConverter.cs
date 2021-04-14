using System;
using System.Linq;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiResourceObjectConverter<T> : WrappedJsonConverter<T>
    {
        private readonly JsonTypeInfo readInfo;

        public JsonApiResourceObjectConverter(JsonTypeInfo readInfo)
        {
            this.readInfo = readInfo;
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
                var name = reader.ReadMemberFast(ref documentState);

                if (name.SequenceEqual(JsonApiMembers.DataEncoded.EncodedUtf8Bytes))
                {
                    resource = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.IncludedEncoded.EncodedUtf8Bytes))
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
            // Really janky way of doing this as it means parsing over
            // included twice if included appears first in the document.
            // This needs to be re-thought.
            if (includedReadFirst)
            {
                ReadIncluded(ref savedReader, ref tracked, options);
            }

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

            var resource = existingValue ?? readInfo.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMemberFast(ref resourceState);

                if (name.IsEqual(JsonApiMembers.IdEncoded) || name.IsEqual(JsonApiMembers.TypeEncoded))
                {
                    readInfo.GetMember(name).Read(ref reader, resource);
                }
                else if (name.IsEqual(JsonApiMembers.AttributesEncoded))
                {
                    reader.ReadObject(JsonApiMemberCode.ResourceAttributes);

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMemberFast(JsonApiMemberCode.Resource);

                        readInfo.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else if (name.IsEqual(JsonApiMembers.MetaEncoded))
                {
                    readInfo.GetMember(name).Read(ref reader, resource);
                }
                else if (name.IsEqual(JsonApiMembers.RelationshipsEncoded))
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
                var relationshipName = reader.ReadMemberFast(JsonApiMemberCode.Relationship);

                readInfo.GetMember(relationshipName).ReadRelationship(ref reader, ref tracked, resource);

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

                var index = 0;

                while (index < tracked.Count)
                {
                    var included = tracked.Get(index);
                    included.Converter.Write(writer, ref tracked, included.Value, options);

                    index++;
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

            // Anonymous objects are cast as object and se we need to dynamically get their type info.
            // However we can skip this step if it's an actual type and just use the cached info.
            var writeInfo = typeof(T) == typeof(object)
                ? options.GetTypeInfo(value.GetType())
                : readInfo;

            ValidateResource(writeInfo);

            writer.WriteStartObject();

            writeInfo.IdMember.Write(writer, ref tracked, value);
            writeInfo.TypeMember.Write(writer, ref tracked, value);

            if (writeInfo.AttributeMembers.Any())
            {
                writer.WritePropertyName(JsonApiMembers.AttributesEncoded);
                writer.WriteStartObject();

                foreach (var member in writeInfo.AttributeMembers)
                {
                    member.Write(writer, ref tracked, value);
                }

                writer.WriteEndObject();
            }

            if (writeInfo.RelationshipMembers.Any())
            {
                var relationshipsWritten = false;

                foreach (var member in writeInfo.RelationshipMembers)
                {
                    member.WriteRelationship(writer, ref tracked, value, ref relationshipsWritten);
                }

                if (relationshipsWritten)
                {
                    writer.WriteEndObject();
                }
            }

            writeInfo.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }

        private void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.IdMember;

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource id must be a string");
            }

            var typeProperty = info.TypeMember;

            if (string.IsNullOrEmpty(typeProperty.Name))
            {
                throw new JsonApiFormatException("JSON:API resource must have a 'type' member");
            }

            if (typeProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource type must be a string");
            }
        }
    }
}
