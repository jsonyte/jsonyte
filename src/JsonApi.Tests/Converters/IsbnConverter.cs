using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Tests.Models;

namespace JsonApi.Tests.Converters
{
    public class IsbnConverter : JsonConverter<Isbn>
    {
        public override Isbn Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            var parts = value.Split('-');

            return new Isbn(parts[0], parts[1], parts[2], parts[3]);
        }

        public override void Write(Utf8JsonWriter writer, Isbn value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value.Part1}-{value.Part2}-{value.Part3}-{value.Part4}");
        }
    }
}
