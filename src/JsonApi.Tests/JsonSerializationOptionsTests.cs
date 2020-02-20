using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests
{
    public class JsonSerializationOptionsTests
    {
        const string Json = @"
            {
              'data': {
                'id': '1',
                'type': 'articles',
                'attributes': {
                  'title': 'Article title'
                }
              }
            }";

        [Fact]
        public void AppendedResourceConverterNeverCalled()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            var converter = new ResourceConverter();
            options.Converters.Add(converter);

            Json.Deserialize<Article>(options);

            Assert.False(converter.ReadCalled);
        }

        [Fact]
        public void InsertedResourceConverterIsCalled()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            var converter = new ResourceConverter();
            options.Converters.Insert(0, converter);

            Json.Deserialize<Article>(options);

            Assert.True(converter.ReadCalled);
        }

        private class ResourceConverter : JsonConverter<Article>
        {
            public bool ReadCalled { get; private set; }

            public override Article Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                ReadCalled = true;

                reader.Skip();

                return null;
            }

            public override void Write(Utf8JsonWriter writer, Article value, JsonSerializerOptions options)
            {
            }
        }
    }
}
