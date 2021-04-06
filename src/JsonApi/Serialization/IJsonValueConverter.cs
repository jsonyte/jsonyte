using System.Text.Json;

namespace JsonApi.Serialization
{
    internal interface IJsonValueConverter
    {
        void Read(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue, JsonSerializerOptions options);
    }
}
