using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Serialization.Reflection;

namespace Jsonyte.Converters
{
    internal class JsonApiStateConverter : JsonConverter<JsonApiStateConverter>
    {
        public ConcurrentDictionary<Type, JsonTypeInfo> Types { get; } = new();

        public ConcurrentDictionary<Type, JsonObjectConverter> ObjectConverters { get; } = new();

        public ConcurrentDictionary<Type, AnonymousRelationshipConverter> AnonymousConverters { get; } = new();

#if NETCOREAPP || NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER
        public MemberAccessor MemberAccessor { get; } = new EmitMemberAccessor();
#else
        public MemberAccessor MemberAccessor { get; } = new ReflectionMemberAccessor();
#endif

        public override JsonApiStateConverter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, JsonApiStateConverter value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            if (!Types.TryGetValue(type, out var value))
            {
                return Types.GetOrAdd(type, x => new JsonTypeInfo(x, options));
            }

            return value;
        }
    }
}
