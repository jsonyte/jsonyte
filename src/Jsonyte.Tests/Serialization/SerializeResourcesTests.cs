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

        [Fact]
        public void CanSerializeCollectionWithCircularReferences()
        {
            var model1 = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here1"
            };

            var model2 = new ModelWithCircularType
            {
                Id = "2",
                Type = "first",
                Value = "here2"
            };

            var another1 = new ModelWithAnotherCircularType
            {
                Id = "3",
                Type = "second",
                Value = "we1",
                Second = model1
            };

            var another2 = new ModelWithAnotherCircularType
            {
                Id = "4",
                Type = "second",
                Value = "we2",
                Second = model2
            };

            model1.First = another1;
            model2.First = another2;

            var models = new[] { model1, model2 };

            var json = models.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'first',
                      'attributes': {
                        'value': 'here1'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '3',
                            'type': 'second'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'first',
                      'attributes': {
                        'value': 'here2'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '4',
                            'type': 'second'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '3',
                      'type': 'second',
                      'attributes': {
                        'value': 'we1'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    },
                    {
                      'id': '4',
                      'type': 'second',
                      'attributes': {
                        'value': 'we2'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '2',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCollectionWithCircularReferencesWithSameIdAndType()
        {
            var model1 = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here1",
                First = new ModelWithAnotherCircularType
                {
                    Id = "3",
                    Type = "second",
                    Value = "we1",
                    Second = new ModelWithCircularType
                    {
                        Id = "1",
                        Type = "first"
                    }
                }
            };

            var model2 = new ModelWithCircularType
            {
                Id = "2",
                Type = "first",
                Value = "here2",
                First = new ModelWithAnotherCircularType
                {
                    Id = "4",
                    Type = "second",
                    Value = "we2",
                    Second = new ModelWithCircularType
                    {
                        Id = "2",
                        Type = "first"
                    }
                }
            };

            var models = new[] { model1, model2 };

            var json = models.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'first',
                      'attributes': {
                        'value': 'here1'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '3',
                            'type': 'second'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'first',
                      'attributes': {
                        'value': 'here2'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '4',
                            'type': 'second'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '3',
                      'type': 'second',
                      'attributes': {
                        'value': 'we1'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    },
                    {
                      'id': '4',
                      'type': 'second',
                      'attributes': {
                        'value': 'we2'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '2',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeRelationshipsFromCollectionInterface()
        {
            var model = new ModelWithCollectionInterfaceRelationships();
            model.Id = "1";
            model.Type = "type";
            model.Articles = new List<Article>
            {
                new()
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Jsonapi"
                },
                new()
                {
                    Id = "3",
                    Type = "articles",
                    Title = "More"
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'type',
                    'relationships': {
                      'articles': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'articles'
                          },
                          {
                            'id': '3',
                            'type': 'articles'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi'
                      }
                    },
                    {
                      'id': '3',
                      'type': 'articles',
                      'attributes': {
                        'title': 'More'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceWithAttributeArray()
        {
            var articles = new ModelWithAttribute[]
            {
                new()
                {
                    Id = "1",
                    Value = "Book 1",
                    IntValue = 1
                },
                new()
                {
                    Id = "2",
                    Value = "Book 2",
                    IntValue = 2
                }
            };

            var json = articles.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'model-with-attribute',
                      'attributes': {
                        'value': 'Book 1',
                        'intValue': 1
                      }
                    },
                    {
                      'id': '2',
                      'type': 'model-with-attribute',
                      'attributes': {
                        'title': 'Book 2',
                        'intValue': 2
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceWithAttributeAndRelationshipArray()
        {
            var subModels = new[]
            {
                new ModelWithAttribute
                {
                    Id = "1",
                    Value = "First Model",
                    IntValue = 12
                },
                new ModelWithAttribute
                {
                    Id = "12",
                    Value = "Second Model",
                    IntValue = 30
                }
            };

            var model = new ModelWithAttributeAndArray
            {
                Id = "45",
                Title = "The Title",
                AssociatedObjects = subModels
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '45',
                    'type': 'model-attribute-array',
                    'attributes': {
                        'title': 'The Title'
                    },
                    'relationships': {
                      'associatedObjects': {
                        'data': [
                          {
                            'id': '1',
                            'type': 'model-with-attribute'
                          },
                          {
                            'id': '12',
                            'type': 'model-with-attribute'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '1',
                      'type': 'model-with-attribute',
                      'attributes': {
                        'value': 'First Model',
                        'intValue': 12
                      }
                    },
                    {
                      'id': '12',
                      'type': 'model-with-attribute',
                      'attributes': {
                        'value': 'Second Model',
                        'intValue': 30
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
