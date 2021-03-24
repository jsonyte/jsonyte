using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiStateConverter : JsonConverter<JsonApiStateConverter>
    {
        public ConcurrentDictionary<Type, JsonTypeInfo> Classes { get; } = new();

#if NETCOREAPP || NETFRAMEWORK
        public IMemberAccessor MemberAccessor { get; } = new EmitMemberAccessor();
#else
        public IMemberAccessor MemberAccessor { get; } = new ReflectionMemberAccessor();
#endif

        public override JsonApiStateConverter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiStateConverter value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
