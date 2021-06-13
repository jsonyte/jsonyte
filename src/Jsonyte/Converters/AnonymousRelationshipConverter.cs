using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;

namespace Jsonyte.Converters
{
    internal abstract class AnonymousRelationshipConverter
    {
        public abstract void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options);

        public abstract void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options);
    }

    internal class AnonymousRelationshipConverter<T> : AnonymousRelationshipConverter
    {
        public AnonymousRelationshipConverter(JsonSerializerOptions options)
        {
            Converter = options.GetRelationshipConverter<T>();
        }

        public JsonApiRelationshipDetailsConverter<T> Converter { get; }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options)
        {
            Converter.Write(writer, ref tracked, new RelationshipResource<T>((T) value), options);
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options)
        {
            Converter.WriteWrapped(writer, ref tracked, new RelationshipResource<T>((T) value), options);
        }
    }
}
