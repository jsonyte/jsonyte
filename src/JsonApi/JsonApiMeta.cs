using System.Collections.Generic;
using System.Text.Json;

namespace JsonApi
{
    public class JsonApiMeta : Dictionary<string, JsonElement>
    {
        public static JsonElement Create(object value, JsonSerializerOptions? options = null)
        {
            return Create(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        public static JsonElement Create<T>(T value, JsonSerializerOptions? options = null)
        {
            return Create(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        private static JsonElement Create(byte[] value)
        {
            using var document = JsonDocument.Parse(value);

            return document.RootElement.Clone();
        }
    }
}
