using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal abstract class WrappedJsonConverter<T> : JsonConverter<T>
    {
        public abstract T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options);

        public abstract void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options);
    }
}
