using System;
using System.Text.Json;

namespace JsonApi.Helpers
{
    public static class Prototype
    {
        public static JsonDocument JsonDocumentFromObject<TValue>(TValue value, JsonSerializerOptions options = default)
        {
            return JsonDocumentFromObject(value, typeof(TValue), options);
        }

        public static JsonDocument JsonDocumentFromObject(object value, Type type, JsonSerializerOptions options = default)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);

            return JsonDocument.Parse(bytes);
        }

        public static JsonElement JsonElementFromObject<TValue>(TValue value, JsonSerializerOptions options = default)
        {
            return JsonElementFromObject(value, typeof(TValue), options);
        }

        public static JsonElement JsonElementFromObject(object value, Type type, JsonSerializerOptions options = default)
        {
            using var doc = JsonDocumentFromObject(value, type, options);

            return doc.RootElement.Clone();
        }
    }
}
