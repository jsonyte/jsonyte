using System.Text.Json;

namespace JsonApi.Tests
{
    public static class ObjectExtensions
    {
        public static string Serialize<T>(this T value)
        {
            return JsonSerializer.Serialize(value, CreateOptions());
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            return options;
        }
    }
}
