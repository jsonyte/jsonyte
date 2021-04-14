using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal interface IWrappedObjectConverter
    {
        void WriteWrappedObject(Utf8JsonWriter writer, ref TrackedResources tracked, object? value, JsonSerializerOptions options);
    }
}
