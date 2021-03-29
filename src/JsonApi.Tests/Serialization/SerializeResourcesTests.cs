using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeResourcesTests
    {
        [Fact]
        public void CanSerializeResourceArray()
        {
            var articles = new Article[]
            {
                new()
                {
                    Id = "1",
                    Type = "articles",
                    Title = "Book 1"
                },
                new()
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Book 2"
                }
            };

            var json = articles.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Book 1'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Book 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeEmptyArray()
        {
            var articles = new Article[0];

            var json = articles.Serialize();

            Assert.Equal(@"
                {
                  'data': []
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
