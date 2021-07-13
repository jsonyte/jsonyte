using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiResourceObjectConverter<T> : WrappedResourceJsonConverter<T>
    {
        private JsonTypeInfo? info;

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

                if (name == DocumentFlags.Data)
                {
                    resource = ReadWrapped(ref reader, ref tracked, default, options);
                }
                else if (name == DocumentFlags.Included)
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

            if (includedReadFirst)
            {
                ReadIncluded(ref savedReader, ref tracked, options);
            }

            state.Validate();

            return resource;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            EnsureTypeInfo(options);

            var state = reader.ReadResource();

            var resource = existingValue ?? info!.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == ResourceFlags.Id)
                {
                    info!.IdMember.Read(ref reader, resource);
                }
                else if (name == ResourceFlags.Type)
                {
                    info!.TypeMember.Read(ref reader, resource);
                }
                else if (name == ResourceFlags.Attributes)
                {
                    reader.ReadObject(JsonApiMemberCode.ResourceAttributes);

                    while (reader.IsInObject())
                    {
                        var attributeName = reader.ReadMember(JsonApiMemberCode.Resource);

                        info!.GetMember(attributeName).Read(ref reader, resource);

                        reader.Read();
                    }
                }
                else if (name == ResourceFlags.Meta)
                {
                    info!.MetaMember.Read(ref reader, resource);
                }
                else if (name == ResourceFlags.Relationships)
                {
                    ReadRelationships(ref reader, ref tracked, resource);
                }
                else if (name == ResourceFlags.Links)
                {
                    info!.LinksMember.Read(ref reader, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            if (info!.HasCircularReferences)
            {
                CacheResource(ref tracked, (T) resource, options);
            }

            return (T) resource;
        }

        private void ReadRelationships(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            reader.ReadObject(JsonApiMemberCode.Relationship);

            while (reader.IsInObject())
            {
                var relationshipName = reader.ReadMember(JsonApiMemberCode.Relationship);

                info!.GetMember(relationshipName).ReadRelationship(ref reader, ref tracked, resource);

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

        private void CacheResource(ref TrackedResources tracked, T? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return;
            }

            var id = info!.IdMember.GetValue(value) as string;
            var type = info.TypeMember.GetValue(value) as string;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                return;
            }

            tracked.SetIncluded(new ResourceIdentifier(id!.ToByteArray(), type!.ToByteArray()), id!, type!, options.GetObjectConverter<T>(), value);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            EnsureTypeInfo(options);

            var tracked = new TrackedResources();

            writer.WriteStartObject();

            writer.WritePropertyName(JsonApiMembers.DataEncoded);
            WriteWrapped(writer, ref tracked, value, options);
            
            if (tracked.Count > 0)
            {
                var nameWritten = false;
                var index = 0;

                while (index < tracked.Count)
                {
                    var included = tracked.Get(index);

                    if (!tracked.IsEmitted(included))
                    {
                        if (!nameWritten)
                        {
                            writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                            writer.WriteStartArray();

                            nameWritten = true;
                        }

                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }

                    index++;
                }

                if (nameWritten)
                {
                    writer.WriteEndArray();
                }
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

            EnsureTypeInfo(options);

            writer.WriteStartObject();

            var idWritten = info!.IdMember.Write(writer, ref tracked, value);
            var typeWritten = info.TypeMember.Write(writer, ref tracked, value);

            SetEmitted(ref tracked, value, idWritten, typeWritten);

            WriteAttributes(writer, ref tracked, value);
            WriteRelationships(writer, ref tracked, value);

            info.LinksMember.Write(writer, ref tracked, value);
            info.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }

        private void WriteAttributes(Utf8JsonWriter writer, ref TrackedResources tracked, T value)
        {
            var attributesWritten = false;

            foreach (var member in info!.AttributeMembers)
            {
                if (member.IsRelationship)
                {
                    continue;
                }

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

            foreach (var member in info!.AttributeMembers)
            {
                if (member.IsRelationship)
                {
                    member.WriteRelationship(writer, ref tracked, value!, ref relationshipsWritten);
                }
            }

            for (var i = 0; i < tracked.Relationships.Count; i++)
            {
                var relationship = tracked.Relationships.Get(i);

                if (!relationshipsWritten)
                {
                    writer.WritePropertyName(JsonApiMembers.RelationshipsEncoded);
                    writer.WriteStartObject();

                    relationshipsWritten = true;
                }

                writer.WritePropertyName(relationship.Name);
                writer.WriteStartObject();

                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                writer.WriteStartArray();

                for (var j = 0; j < tracked.Count; j++)
                {
                    var included = tracked.Get(j);

                    if (included.IsUnwritten() && included.RelationshipId == relationship.Id)
                    {
                        writer.WriteStartObject();

                        writer.WriteString(JsonApiMembers.IdEncoded, included.Identifier.Id);
                        writer.WriteString(JsonApiMembers.TypeEncoded, included.Identifier.Type);

                        writer.WriteEndObject();
                    }
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            if (relationshipsWritten)
            {
                writer.WriteEndObject();
            }

            tracked.Relationships.Clear();
        }

        private void SetEmitted(ref TrackedResources tracked, T value, bool idWritten, bool typeWritten)
        {
            if (idWritten && typeWritten)
            {
                var id = info!.IdMember.GetValue(value!) as string;
                var type = info.TypeMember.GetValue(value!) as string;

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(type))
                {
                    tracked.SetEmitted(id!, type!);
                }
            }
        }

        private void EnsureTypeInfo(JsonSerializerOptions options)
        {
            if (info == null)
            {
                info = options.GetTypeInfo(typeof(T));

                ValidateResource(info);
            }
        }

        private void ValidateResource(JsonTypeInfo typeInfo)
        {
            var idProperty = typeInfo.IdMember;

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource id must be a string");
            }

            var typeProperty = typeInfo.TypeMember;

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
