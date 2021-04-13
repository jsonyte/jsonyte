using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte
{
    public sealed class JsonApiMeta : Dictionary<string, JsonElement>
    {
        public static JsonElement Value(object value, JsonSerializerOptions? options = null)
        {
            return Value(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        public static JsonElement Value<T>(T value, JsonSerializerOptions? options = null)
        {
            return Value(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        private static JsonElement Value(byte[] value)
        {
            using var document = JsonDocument.Parse(value);

            return document.RootElement.Clone();
        }
    }
}
