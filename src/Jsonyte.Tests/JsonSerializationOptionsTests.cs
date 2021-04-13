using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests
{
    public class JsonSerializationOptionsTests
    {
        private const string Json = @"
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
            var converter = new ResourceConverter();

            options.AddJsonApi();
            options.Converters.Add(converter);

            Json.Deserialize<Article>(options);

            Assert.False(converter.ReadCalled);
        }

        [Fact]
        public void InsertedResourceConverterIsCalled()
        {
            var options = new JsonSerializerOptions();
            var converter = new ResourceConverter();

            options.AddJsonApi();
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
