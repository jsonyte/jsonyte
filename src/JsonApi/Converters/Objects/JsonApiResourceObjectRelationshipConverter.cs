using System;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Objects
{
    internal class JsonApiResourceObjectRelationshipConverter<T> : JsonApiRelationshipDetailsConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override T? Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options)
        {
            var relationship = default(T);

            var relationshipState = reader.ReadRelationship();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref relationshipState);

                if (name == JsonApiMembers.Data)
                {
                    relationship = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            relationshipState.Validate();

            return relationship;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            var identifier = reader.Read<JsonApiResourceIdentifier>(options);

            if (tracked.TryGetIncluded(identifier, out var included))
            {
                return (T) included.Value;
            }

            var info = options.GetTypeInfo(typeToConvert);
            var relationship = info.Creator();

            if (relationship == null)
            {
                return default;
            }

            info.GetMember(JsonApiMembers.Id).SetValue(relationship, identifier.Id);
            info.GetMember(JsonApiMembers.Type).SetValue(relationship, identifier.Type);

            var converter = options.GetValueConverter<T>();

            tracked.SetIncluded(identifier, converter, relationship);

            return (T) relationship;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.Data);

            WriteWrapped(writer, ref tracked, value, options);

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();

                return;
            }

            var info = options.GetTypeInfo(typeof(T));

            var id = info.GetMember(JsonApiMembers.Id).GetValue(value) as string;
            var type = info.GetMember(JsonApiMembers.Type).GetValue(value) as string;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                throw new JsonApiException($"JSON:API relationship for '{typeof(T).Name}' must have both 'id' and 'type' values");
            }

            writer.WriteStartObject();

            writer.WriteString(JsonApiMembers.Id, id);
            writer.WriteString(JsonApiMembers.Type, type);

            writer.WriteEndObject();

            tracked.SetIncluded(new JsonApiResourceIdentifier(id!, type!), options.GetValueConverter<T>(), value);
        }
    }
}
