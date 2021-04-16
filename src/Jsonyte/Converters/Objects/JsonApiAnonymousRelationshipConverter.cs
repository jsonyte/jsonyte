using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Objects
{
    internal class JsonApiAnonymousRelationshipConverter<T> : IAnonymousRelationshipConverter
    {
        private readonly JsonSerializerOptions options;

        public JsonApiAnonymousRelationshipConverter(JsonSerializerOptions options)
        {
            this.options = options;

            Converter = options.GetRelationshipConverter<T>();
        }

        public JsonApiRelationshipDetailsConverter<T> Converter { get; }

        public void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value)
        {
            Converter.Write(writer, ref tracked, new RelationshipResource<T>((T) value), options);
        }
    }
}
