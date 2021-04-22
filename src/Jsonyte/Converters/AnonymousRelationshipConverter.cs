using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal abstract class AnonymousRelationshipConverter
    {
        public abstract void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value);

        public abstract void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object value);
    }
}
