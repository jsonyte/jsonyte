using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
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

        [Fact]
        public void CanSerializeAnonymousArray()
        {
            var articles = new[]
            {
                new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi"
                },
                new
                {
                    id = "2",
                    type = "articles",
                    title = "Jsonapi 2"
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
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeArrayInDocument()
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
                    },
                    new JsonApiResource
                    {
                        Id = "2",
                        Type = "articles",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"title", "Jsonapi 2".ToElement()}
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeArrayInTypedDocument()
        {
            var document = new JsonApiDocument<Article[]>
            {
                Data = new Article[]
                {
                    new()
                    {
                        Id = "1",
                        Type = "articles",
                        Title = "Jsonapi"
                    },
                    new()
                    {
                        Id = "2",
                        Type = "articles",
                        Title = "Jsonapi 2"
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
