using System;
using System.Collections;
using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Collections
{
    internal class JsonApiPotentialRelationshipCollectionConverter : WrappedJsonConverter<PotentialRelationshipCollection>
    {
        public override PotentialRelationshipCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override PotentialRelationshipCollection ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, PotentialRelationshipCollection existingValue, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, PotentialRelationshipCollection value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, PotentialRelationshipCollection container, JsonSerializerOptions options)
        {
            if (container.Value is not IEnumerable enumerable)
            {
                if (container.WriteImmediately)
                {
                    throw new JsonApiException("JSON:API relationship collection must be enumerable");
                }

                return;
            }

            TypeDescriptor? descriptor = null;
            var writtenProperty = false;

            if (container.WriteImmediately)
            {
                writer.WriteStartArray();
            }

            foreach (var value in enumerable)
            {
                descriptor ??= GetValueDescriptor(value, container, options);

                if (descriptor.Value.Info == null || descriptor.Value.Converter == null)
                {
                    if (!writtenProperty)
                    {
                        writer.WritePropertyName(container.Relationship);
                        writer.WriteStartArray();

                        writtenProperty = true;
                    }

                    WriteDefault(writer, value, descriptor.Value.ValueType, options);
                }
                else
                {
                    if (value == null)
                    {
                        throw new JsonApiFormatException("JSON:API relationship object cannot be null");
                    }

                    WriteResource(writer, ref tracked, descriptor.Value.Info, container, value, descriptor.Value.Converter);
                }
            }

            if (container.WriteImmediately || writtenProperty)
            {
                writer.WriteEndArray();
            }

            tracked.Relationships.LastWritten = container.WriteImmediately || writtenProperty;
        }

        private TypeDescriptor GetValueDescriptor(object? value, PotentialRelationshipCollection container, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return default;
            }

            var valueType = value.GetType();

            if (valueType.IsResourceIdentifier())
            {
                var info = options.GetTypeInfo(valueType);
                var converter = options.GetObjectConverter(valueType);

                return new TypeDescriptor(valueType, info, converter);
            }

            if (container.WriteImmediately)
            {
                throw new JsonApiException("JSON:API relationship resources must have string 'id' and 'type' members");
            }

            return new TypeDescriptor(valueType);
        }

        private void WriteDefault(Utf8JsonWriter writer, object? value, Type? valueType, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, valueType ?? typeof(object), options);
        }

        private void WriteResource(
            Utf8JsonWriter writer,
            ref TrackedResources tracked,
            JsonTypeInfo info,
            PotentialRelationshipCollection container,
            object value,
            IJsonObjectConverter converter)
        {
            var id = info.IdMember.GetValue(value) as string;
            var type = info.TypeMember.GetValue(value) as string;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                throw new JsonApiFormatException("JSON:API resources must have string 'id' and 'type' members");
            }

            var idEncoded = JsonEncodedText.Encode(id!);
            var typeEncoded = JsonEncodedText.Encode(type!);

            var identifier = new ResourceIdentifier(idEncoded.EncodedUtf8Bytes, typeEncoded.EncodedUtf8Bytes);

            if (container.WriteImmediately)
            {
                writer.WriteStartObject();

                writer.WriteString(JsonApiMembers.IdEncoded, idEncoded);
                writer.WriteString(JsonApiMembers.TypeEncoded, typeEncoded);

                writer.WriteEndObject();

                tracked.SetIncluded(identifier, converter, value);
            }
            else
            {
                tracked.SetIncluded(identifier, converter, value, container.Relationship);
            }
        }

        private readonly struct TypeDescriptor
        {
            public readonly Type? ValueType;

            public readonly JsonTypeInfo? Info;

            public readonly IJsonObjectConverter? Converter;

            public TypeDescriptor(Type valueType)
            {
                ValueType = valueType;
                Info = null;
                Converter = null;
            }

            public TypeDescriptor(Type valueType, JsonTypeInfo? info, IJsonObjectConverter? converter)
            {
                ValueType = valueType;
                Info = info;
                Converter = converter;
            }
        }
    }
}
