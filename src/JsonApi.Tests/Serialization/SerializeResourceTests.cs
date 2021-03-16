using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeResourceTests
    {
        [Fact(Skip = "Not implemented")]
        public void CanSerializeResourceObject()
        {
            var article = new Article
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi"
            };

            var json = article.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }
    }
}
