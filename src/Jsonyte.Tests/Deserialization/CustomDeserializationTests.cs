using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Deserialization
{
    public class CustomDeserializationTests
    {
        [Fact]
        public void CanDeserializeWithCustomConverterAttribute()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book',
                      'isbn': '1231-4561-4587-0'
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithIsbnAndConverter>();

            Assert.Equal("1231", article.Isbn.Part1);
            Assert.Equal("4561", article.Isbn.Part2);
            Assert.Equal("4587", article.Isbn.Part3);
            Assert.Equal("0", article.Isbn.Part4);
        }
    }
}
