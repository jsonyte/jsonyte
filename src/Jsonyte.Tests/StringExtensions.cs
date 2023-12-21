using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json;

namespace Jsonyte.Tests
{
    public static class StringExtensions
    {
        public static string EncodeQuery(this string value)
        {
            var parameters = new NameValueCollection();

            var parts = value.TrimStart('?').Split('&');

            foreach (var part in parts)
            {
                var keyValue = part.Split('=');

                parameters.Add(Uri.EscapeDataString(keyValue[0]), Uri.EscapeDataString(keyValue[1]));
            }

            return $"?{string.Join("&", parameters.AllKeys.Select(x => $"{x}={parameters[x]}"))}";
        }

        public static string Format(this string value)
        {
            return value.TrimIndent().ReplaceQuotes();
        }

        public static string TrimIndent(this string value)
        {
            var lines = value.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            var indentWidth = lines
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.IndentWidth())
                .Min();

            var newLines = lines
                .Select(x => string.IsNullOrEmpty(x)
                    ? string.Empty
                    : x.Substring(indentWidth))
                .ToArray();

            return string.Join(Environment.NewLine, newLines);
        }

        public static int IndentWidth(this string value)
        {
            return value
                .TakeWhile(char.IsWhiteSpace)
                .Count();
        }

        public static string ReplaceQuotes(this string value)
        {
            return value.Replace('\'', '\"');
        }

        public static T Deserialize<T>(this string value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(value.ReplaceQuotes(), CreateOptions(options));
        }

        public static object Deserialize(this string value, Type type, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize(value.ReplaceQuotes(), type, CreateOptions(options));
        }

        public static MockJsonApiDocument DeserializeDocument(this string value, Type type, JsonSerializerOptions options = null)
        {
            var document = new MockJsonApiDocument();

            var frameworkDocument = value.Deserialize(type, options);

            document.Data = frameworkDocument.GetValue(nameof(JsonApiDocument.Data));
            document.Errors = frameworkDocument.GetValue<JsonApiError[]>(nameof(JsonApiDocument.Errors));
            document.Meta = frameworkDocument.GetValue<JsonApiMeta>(nameof(JsonApiDocument.Meta));
            document.JsonApi = frameworkDocument.GetValue<JsonApiObject>(nameof(JsonApiDocument.JsonApi));
            document.Links = frameworkDocument.GetValue<JsonApiDocumentLinks>(nameof(JsonApiDocument.Links));
            document.Included = frameworkDocument.GetValue<JsonApiResource[]>(nameof(JsonApiDocument.Included));

            return frameworkDocument == null
                ? null
                : document;
        }

        private static JsonSerializerOptions CreateOptions(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();
            options.AddJsonApi();

            return options;
        }
    }
}
