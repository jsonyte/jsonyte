using System;
using System.Text.Json;
using JsonApiSerializer;
using Jsonyte.Tests.Converters;
using Jsonyte.Tests.Models;
using Newtonsoft.Json;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class CustomSerializationTests
    {
        [Fact]
        public void CanSerializeWithCustomConverterAttribute()
        {
            var article = new ArticleWithIsbnAndConverter
            {
                Id = "1",
                Type = "articles",
                Title = "book",
                Isbn = new Isbn("1231", "4561", "4587", "0")
            };

            var json = article.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book',
                      'isbn': '1231-4561-4587-0'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void TimeSpanConverterCanParseLikeNewtonsoft()
        {
            var model = new
            {
                id = "1",
                type = "type",
                time = TimeSpan.FromSeconds(265)
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new TimeSpanConverter());

            var settings = new JsonApiSerializerSettings();

            var json = model.Serialize(options);

            Assert.Equal(JsonConvert.SerializeObject(model, settings), json, JsonStringEqualityComparer.Default);
        }
    }
}
