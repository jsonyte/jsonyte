using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
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
    }
}
