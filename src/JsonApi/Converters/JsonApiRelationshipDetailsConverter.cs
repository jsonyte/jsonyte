using System;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal abstract class JsonApiRelationshipDetailsConverter<T> : WrappedJsonConverter<T>
    {
        public abstract T? Read(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, JsonSerializerOptions options);

        public abstract void Write(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);
    }
}
