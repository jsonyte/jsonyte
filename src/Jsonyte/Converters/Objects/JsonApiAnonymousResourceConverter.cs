using System;
using System.Text;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiAnonymousResourceConverter : WrappedJsonConverter<AnonymousResource>
    {
        public override AnonymousResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override AnonymousResource ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, AnonymousResource existingValue, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, AnonymousResource value, JsonSerializerOptions options)
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

                    if (!tracked.HasResource(Encoding.UTF8.GetString(included.Id), Encoding.UTF8.GetString(included.Type)))
                    {
                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }

                    index++;
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, AnonymousResource container, JsonSerializerOptions options)
        {
            var value = container.Value;

            if (value == null)
            {
                writer.WriteNullValue();

                return;
            }

            var info = options.GetTypeInfo(value.GetType());

            ValidateResource(info);

            writer.WriteStartObject();

            var idWritten = info.IdMember.Write(writer, ref tracked, value);
            var typeWritten = info.TypeMember.Write(writer, ref tracked, value);

            if (idWritten && typeWritten)
            {
                var id = info.IdMember.GetValue(value) as string;
                var type = info.TypeMember.GetValue(value) as string;

                tracked.SetResource(id!, type!);
            }

            WriteAttributes(writer, info, ref tracked, value);
            WriteRelationships(writer, info, ref tracked, value);

            info.LinksMember.Write(writer, ref tracked, value);
            info.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }

        private void WriteAttributes(Utf8JsonWriter writer, JsonTypeInfo info, ref TrackedResources tracked, object value)
        {
            var attributesWritten = false;

            foreach (var member in info.AttributeMembers)
            {
                var memberName = attributesWritten
                    ? default
                    : JsonApiMembers.AttributesEncoded;

                attributesWritten |= member.Write(writer, ref tracked, value, memberName);
            }

            if (attributesWritten)
            {
                writer.WriteEndObject();
            }
        }

        private void WriteRelationships(Utf8JsonWriter writer, JsonTypeInfo info, ref TrackedResources tracked, object value)
        {
            var relationshipsWritten = false;

            foreach (var member in info.AttributeMembers)
            {
                if (member.IsRelationship)
                {
                    member.WriteRelationship(writer, ref tracked, value, ref relationshipsWritten);
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
