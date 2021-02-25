using System;
using System.Text.Json;

namespace JsonApi.Tests
{
    public static class StringExtensions
    {
        public static string ToDoubleQuoted(this string value)
        {
            return value.Replace('\'', '\"');
        }

        public static string Format(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        public static T Deserialize<T>(this string value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(value.ToDoubleQuoted(), options ?? CreateOptions());
        }

        public static object Deserialize(this string value, Type type, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize(value.ToDoubleQuoted(), type, options ?? CreateOptions());
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            return options;
        }
    }
}
