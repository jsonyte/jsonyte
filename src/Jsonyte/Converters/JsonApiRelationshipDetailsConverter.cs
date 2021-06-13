using System;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;

namespace Jsonyte.Converters
{
    internal abstract class JsonApiRelationshipDetailsConverter<T> : WrappedJsonConverter<RelationshipResource<T>>
    {
        public abstract RelationshipResource<T> Read(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options);

        public abstract void Write(Utf8JsonWriter writer, ref TrackedResources tracked, RelationshipResource<T> value, JsonSerializerOptions options);
    }
}
