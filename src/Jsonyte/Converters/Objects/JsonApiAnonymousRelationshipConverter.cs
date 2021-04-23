using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Contracts;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiAnonymousRelationshipConverter<T> : AnonymousRelationshipConverter
    {
        private readonly JsonSerializerOptions options;

        public JsonApiAnonymousRelationshipConverter(JsonSerializerOptions options)
        {
            this.options = options;

            Converter = options.GetRelationshipConverter<T>();
        }

        public JsonApiRelationshipDetailsConverter<T> Converter { get; }

        public override void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value)
        {
            Converter.Write(writer, ref tracked, new RelationshipResource<T>((T) value), options);
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object value)
        {
            Converter.WriteWrapped(writer, ref tracked, new RelationshipResource<T>((T) value), options);
        }
    }
}
