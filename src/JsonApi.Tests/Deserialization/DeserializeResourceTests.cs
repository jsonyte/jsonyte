using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeResourceTests : ValidationTests
    {
        [Fact]
        public void CanDeserializeSimpleObject()
        {
            const string json = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi'
                }
              }
            }";

            var document = json.Deserialize<JsonApiDocument<Article>>();
            var article = document.Data;

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact]
        public void CanDeserializeSimpleArray()
        {
            const string json = @"
            {
              'data': [{
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'Jsonapi'
                }
              },
              {
                'type': 'articles',
                'id': '2',
                'attributes': {
                  'title': 'Jsonapi 2'
                }
              }]
            }";

            var document = json.Deserialize<JsonApiDocument<Article[]>>();
            var article = document.Data;
        }
    }
}
