using System.Text.Json;

namespace JsonApi.Serialization
{
    internal interface IJsonPropertyInfo
    {
        void Read(ref Utf8JsonReader reader, object resource);

        void Write(Utf8JsonWriter writer, object resource);
    }
}
