using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeResourceTests
    {
        [Fact]
        public void CanSerializeResource()
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceInDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                Data = new Article
                {
                    Id = "1",
                    Type = "articles",
                    Title = "Jsonapi"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceInPlainDocument()
        {
            var document = new JsonApiDocument
            {
                Data = new[]
                {
                    new JsonApiResource
                    {
                        Id = "1",
                        Type = "articles",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"title", "Jsonapi".ToElement()}
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void ResourceIdMustBeAString()
        {
            var resource = new
            {
                id = 1,
                type = "articles",
                title = "Jsonapi"
            };

            var exception = Record.Exception(() => resource.Serialize());

            Assert.NotNull(exception);
            Assert.IsType<JsonApiException>(exception);
        }

        [Fact]
        public void ResourceWithoutTypeIgnored()
        {
            var article = new ModelWithNoType
            {
                Id = "1",
                Title = "Jsonapi"
            };

            var json = article.Serialize();

            Assert.Equal(@"
                {
                  'id': '1',
                  'title': 'Jsonapi'
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeAnonymousResource()
        {
            var resource = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi"
            };

            var json = resource.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
