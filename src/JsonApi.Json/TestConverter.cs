using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class Main
    {
        public Main()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new TestConverter());
        }
    }

    public class TestConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class TestConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            throw new NotImplementedException();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}