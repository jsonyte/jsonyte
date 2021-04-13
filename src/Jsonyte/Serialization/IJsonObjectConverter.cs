using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal interface IJsonObjectConverter
    {
        void Read(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue, JsonSerializerOptions options);

        void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options);
    }
}
