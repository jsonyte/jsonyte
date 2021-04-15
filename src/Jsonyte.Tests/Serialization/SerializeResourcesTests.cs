using System.Collections.Generic;
using System.Linq;
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
        public void CanSerializeResourceWithRelationshipArray()
        {
            var author = new Author
            {
                Id = "4",
                Type = "authors",
                Name = "Bob"
            };

            var articles = new[]
            {
                new ArticleWithAuthor
                {
                    Id = "1",
                    Type = "articles",
                    Title = "Wifi",
                    Author = author
                },
                new ArticleWithAuthor
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Home Theater",
                    Author = author
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
                        'title': 'Wifi'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '4',
                            'type': 'authors'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Home Theater'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '4',
                            'type': 'authors'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '4',
                      'type': 'authors',
                      'attributes': {
                        'name': 'Bob',
                        'twitter': null
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
        public void CanSerializeAnonymousEnumerableCastAsObject()
        {
            IEnumerable<object> GetResources()
            {
                return new[]
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
            }

            var json = JsonApiDocument.Create(GetResources()).Serialize();

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
        public void CanSerializeAnonymousArrayPreservingType()
        {
            object[] GetResources()
            {
                return new[]
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
            }

            var json = JsonApiDocument.Create(GetResources()).Serialize();

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
        public void CanSerializeArrayCastingToObjectAndLosingAnonymousType()
        {
            object[] GetResources()
            {
                return new object[]
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
            }

            var json = JsonApiDocument.Create(GetResources()).Serialize();

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
        public void CanSerializeAnonymousObjectsComingFromInterface()
        {
            var factory = new AnonymousModelFactory();
            var transformer = factory.GetTransformer<Article>();

            var articles = new[]
            {
                new Article
                {
                    Id = "1",
                    Type = "articles",
                    Title = "Wifi"
                },
                new Article
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Home Theater"
                }
            };

            var models = articles.Select(x => transformer.GetModel(x));

            var json = JsonApiDocument.Create(models).Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Wifi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Home Theater'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeAnonymousObjectWithRelationshipFromInterface()
        {
            var factory = new AnonymousModelFactory();
            var transformer = factory.GetTransformer<ArticleWithAuthor>();

            var author = new Author
            {
                Id = "4",
                Type = "authors",
                Name = "Bob"
            };

            var articles = new[]
            {
                new ArticleWithAuthor
                {
                    Id = "1",
                    Type = "articles",
                    Title = "Wifi",
                    Author = author
                },
                new ArticleWithAuthor
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Home Theater",
                    Author = author
                }
            };

            var models = articles.Select(x => transformer.GetModel(x));

            var json = JsonApiDocument.Create(models).Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Wifi'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '4',
                            'type': 'authors'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Home Theater'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '4',
                            'type': 'authors'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '4',
                      'type': 'authors',
                      'attributes': {
                        'name': 'Bob'
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

        [Fact]
        public void CanSerializeMetaInAnonymousArray()
        {
            object[] GetResources()
            {
                return new object[]
                {
                    new
                    {
                        id = "1",
                        type = "articles",
                        title = "Jsonapi",
                        meta = new
                        {
                            count = 1
                        }
                    },
                    new
                    {
                        id = "2",
                        type = "articles",
                        title = "Jsonapi 2",
                        meta = new
                        {
                            count = 1
                        }
                    }
                };
            }

            var json = JsonApiDocument.Create(GetResources()).Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'meta': {
                        'count': 1
                      },
                      'attributes': {
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'meta': {
                        'count': 1
                      },
                      'attributes': {
                        'title': 'Jsonapi 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeLinksInAnonymousArray()
        {
            object[] GetResources()
            {
                return new object[]
                {
                    new
                    {
                        id = "1",
                        type = "articles",
                        title = "Jsonapi",
                        links = new
                        {
                            self = "http://me"
                        }
                    },
                    new
                    {
                        id = "2",
                        type = "articles",
                        title = "Jsonapi 2",
                        links = new
                        {
                            self = "http://me"
                        }
                    }
                };
            }

            var json = JsonApiDocument.Create(GetResources()).Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'links': {
                        'self': 'http://me'
                      },
                      'attributes': {
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'links': {
                        'self': 'http://me'
                      },
                      'attributes': {
                        'title': 'Jsonapi 2'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
