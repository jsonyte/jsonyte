using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal abstract class JsonApiConverter<T> : JsonConverter<T>
    {
        public abstract Type? ElementType { get; }

        public abstract T? ReadWrapped(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, T? existingValue, JsonSerializerOptions options);

        public abstract void WriteWrapped(Utf8JsonWriter writer, T value, JsonSerializerOptions options);
    }
}
