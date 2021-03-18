using System.Text.Json;

namespace JsonApi.Tests
{
    public static class ObjectExtensions
    {
        public static string Serialize<T>(this T value)
        {
            return JsonSerializer.Serialize(value, CreateOptions());
        }

        public static JsonElement ToElement<T>(this T value)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            var document = JsonDocument.Parse(bytes);

            return document.RootElement.Clone();
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.AddJsonApi();

            return options;
        }
    }
}
