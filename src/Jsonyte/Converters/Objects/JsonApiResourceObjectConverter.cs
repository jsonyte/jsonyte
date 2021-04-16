using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
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

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            Utf8JsonReader savedReader = default;
            var includedReadFirst = false;

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name.SequenceEqual(JsonApiMembers.DataEncoded.EncodedUtf8Bytes))
                {
                    resource = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else if (name.SequenceEqual(JsonApiMembers.IncludedEncoded.EncodedUtf8Bytes))
                {
                    if (state.HasFlag(DocumentFlags.Data))
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

            state.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var state = reader.ReadResource();

            var resource = existingValue ?? info.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name.IsEqual(JsonApiMembers.IdEncoded) || name.IsEqual(JsonApiMembers.TypeEncoded))
                {
                    info.GetMember(name).Read(ref reader, resource);
                }
                else if (name.IsEqual(JsonApiMembers.AttributesEncoded))
                {
                    reader.ReadObject(JsonApiMemberCode.ResourceAttributes);

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember(JsonApiMemberCode.Resource);

                        info.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else if (name.IsEqual(JsonApiMembers.MetaEncoded))
                {
                    info.GetMember(name).Read(ref reader, resource);
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

            state.Validate();

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

            writer.WriteStartObject();

            info.IdMember.Write(writer, ref tracked, value);
            info.TypeMember.Write(writer, ref tracked, value);

            WriteAttributes(writer, ref tracked, value);
            WriteRelationships(writer, ref tracked, value);

            info.LinksMember.Write(writer, ref tracked, value);
            info.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }

        private void WriteAttributes(Utf8JsonWriter writer, ref TrackedResources tracked, T value)
        {
            var attributesWritten = false;

            foreach (var member in info.AttributeMembers)
            {
                var memberName = attributesWritten
                    ? default
                    : JsonApiMembers.AttributesEncoded;

                attributesWritten |= member.Write(writer, ref tracked, value!, memberName);
            }

            if (attributesWritten)
            {
                writer.WriteEndObject();
            }
        }

        private void WriteRelationships(Utf8JsonWriter writer, ref TrackedResources tracked, T value)
        {
            var relationshipsWritten = false;

            foreach (var member in info.AttributeMembers)
            {
                if (member.IsRelationship)
                {
                    member.WriteRelationship(writer, ref tracked, value!, ref relationshipsWritten);
                }
            }

            if (relationshipsWritten)
            {
                writer.WriteEndObject();
            }
        }
    }
}
