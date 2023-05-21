using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiResourceObjectExplicitRelationshipConverter<T> : JsonApiRelationshipDetailsConverter<T>
    {
        private JsonTypeInfo? info;

        private JsonApiRelationshipDetailsConverter<T>? relationshipConverter;

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override RelationshipResource<T> Read(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options)
        {
            reader.ReadRelationship();

            var state = new RelationshipState();

            EnsureTypeInfo(options);

            var resource = info!.Creator();

            if (resource == null)
            {
                return default;
            }

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == RelationshipFlags.Data)
                {
                    info.DataMember.ReadRelationshipWrapped(ref reader, ref tracked, resource);
                }
                else if (name == RelationshipFlags.Links)
                {
                    info!.LinksMember.Read(ref reader, resource);
                }
                else if (name == RelationshipFlags.Meta)
                {
                    info!.MetaMember.Read(ref reader, resource);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();

            return new RelationshipResource<T>((T) resource);
        }

        public override RelationshipResource<T> ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, RelationshipResource<T> existingValue, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            WriteWrapped(writer, ref tracked, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options)
        {
            if (value.Resource == null)
            {
                writer.WriteNull(JsonApiMembers.DataEncoded);

                return;
            }

            EnsureTypeInfo(options);

            var data = info!.DataMember.GetValue(value.Resource);
            var meta = info.MetaMember.GetValue(value.Resource);
            var links = info.LinksMember.GetValue(value.Resource);

            if (data == null && links == null && meta == null)
            {
                writer.WriteNull(JsonApiMembers.DataEncoded);

                return;
            }

            if (data != null)
            {
                writer.WritePropertyName(JsonApiMembers.DataEncoded);
                info.DataMember.WriteRelationshipWrapped(writer, ref tracked, value.Resource);
            }

            info.LinksMember.Write(writer, ref tracked, value.Resource);
            info.MetaMember.Write(writer, ref tracked, value.Resource);
        }

        private void EnsureTypeInfo(JsonSerializerOptions options)
        {
            info ??= options.GetTypeInfo(typeof(T));
        }

        private JsonApiRelationshipDetailsConverter<T> GetRelationshipConverter(JsonSerializerOptions options)
        {
            return relationshipConverter ??= options.GetRelationshipConverter<T>();
        }
    }
}
