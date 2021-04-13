using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeRelationshipTests
    {
        [Fact]
        public void CanDeserializeSingleRelationship()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Rails is Omakase'
                    },
                    'relationships': {
                      'author': {
                        'links': {
                          'self': '/articles/1/relationships/author',
                          'related': '/articles/1/author'
                        },
                        'data': {
                          'type': 'people',
                          'id': '9'
                        }
                      }
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithAuthor>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Rails is Omakase", article.Title);

            Assert.Equal("people", article.Author.Type);
            Assert.Equal("9", article.Author.Id);
        }
    }
}
