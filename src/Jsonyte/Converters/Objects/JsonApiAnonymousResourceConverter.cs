using System;
using System.Linq;
using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiAnonymousResourceConverter : WrappedJsonConverter<ResourceContainer>
    {
        public override ResourceContainer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override ResourceContainer ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, ResourceContainer existingValue, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ResourceContainer value, JsonSerializerOptions options)
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

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, ResourceContainer container, JsonSerializerOptions options)
        {
            var value = container.Value;

            if (value == null)
            {
                writer.WriteNullValue();

                return;
            }

            var info = options.GetTypeInfo(value.GetType());

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

            info.LinksMember.Write(writer, ref tracked, value);
            info.MetaMember.Write(writer, ref tracked, value);

            writer.WriteEndObject();
        }
    }
}
