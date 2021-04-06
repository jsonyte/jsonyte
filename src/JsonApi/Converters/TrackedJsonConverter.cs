using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal abstract class TrackedJsonConverter<T> : JsonConverter<T>
    {
        public abstract T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options);
    }
}
