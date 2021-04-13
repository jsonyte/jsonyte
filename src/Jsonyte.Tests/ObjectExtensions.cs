using System;
using System.Text.Json;

namespace Jsonyte.Tests
{
    public static class ObjectExtensions
    {
        public static string Serialize<T>(this T value, JsonSerializerOptions options = null)
        {
            return Environment.NewLine + JsonSerializer.Serialize(value, CreateOptions(options));
        }

        public static string SerializeDocument<T>(this T value, Type documentType, JsonSerializerOptions options = null)
            where T : MockJsonApiDocument
        {
            var document = Activator.CreateInstance(documentType);

            document.SetValue(nameof(JsonApiDocument.Data), value.Data);
            document.SetValue(nameof(JsonApiDocument.Errors), value.Errors);
            document.SetValue(nameof(JsonApiDocument.Meta), value.Meta);
            document.SetValue(nameof(JsonApiDocument.JsonApi), value.JsonApi);
            document.SetValue(nameof(JsonApiDocument.Links), value.Links);
            document.SetValue(nameof(JsonApiDocument.Included), value.Included);

            return Environment.NewLine + JsonSerializer.Serialize(document, CreateOptions(options));
        }

        public static object GetValue(this object resource, string name)
        {
            var property = resource?.GetType().GetProperty(name);

            return property?.GetValue(resource);
        }

        public static T GetValue<T>(this object resource, string name)
        {
            return (T) GetValue(resource, name);
        }

        public static void SetValue(this object resource, string name, object value)
        {
            var property = resource.GetType().GetProperty(name);

            property?.SetValue(resource, value);
        }

        public static JsonElement ToElement<T>(this T value)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            var document = JsonDocument.Parse(bytes);

            return document.RootElement.Clone();
        }

        private static JsonSerializerOptions CreateOptions(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();
            options.WriteIndented = true;
            options.AddJsonApi();

            return options;
        }
    }
}
