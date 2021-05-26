using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
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
            Assert.IsType<JsonApiFormatException>(exception);
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

        [Fact]
        public void CanSerializeAnonymousResourceCastAsObject()
        {
            object GetResource()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi"
                };
            }

            var json = GetResource().Serialize();

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

        [Fact]
        public void CanSerializeListOfObjectsInResource()
        {
            var model = new
            {
                id = "1",
                type = "type",
                name = "Bob",
                measurements = new object[] {1, "five", 7}
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'type',
                    'attributes': {
                      'name': 'Bob',
                      'measurements': [1, 'five', 7]
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaAndLinksUsingGenericObject()
        {
            var model = new ArticleWithMetaAndLinks
            {
                Id = "1",
                Type = "type",
                Title = "Jsonapi",
                Links = new Dictionary<string, string>
                {
                    {"self", "http://me"}
                },
                Meta = new Dictionary<string, object>
                {
                    {"count", 1}
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'type',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'links': {
                      'self': 'http://me'
                    },
                    'meta': {
                      'count': 1
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaAndLinksFromAnonymousObject()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                links = new
                {
                    self = "http://me"
                },
                meta = new
                {
                    count = 1
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'links': {
                      'self': 'http://me'
                    },
                    'meta': {
                      'count': 1
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeNumbersAsStrings()
        {
            var model = new ModelWithNullableTypes
            {
                Id = "1",
                Type = "model",
                ByteValue = 1,
                SbyteValue = 2,
                DecimalValue = 3,
                ShortValue = 4,
                UshortValue = 5,
                IntValue = 6,
                UintValue = 7,
                LongValue = 8,
                UlongValue = 9,
                FloatValue = 10,
                DoubleValue = 11,
                ObjectValue = 12
            };

            var options = new JsonSerializerOptions();
            options.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'byteValue': '1',
                      'sbyteValue': '2',
                      'decimalValue': '3',
                      'shortValue': '4',
                      'ushortValue': '5',
                      'intValue': '6',
                      'uintValue': '7',
                      'longValue': '8',
                      'ulongValue': '9',
                      'floatValue': '10',
                      'doubleValue': '11',
                      'objectValue': '12'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceWithAnonymousAttributes()
        {
            object GetNestedAuthor()
            {
                return new
                {
                    name = "Bob",
                    age = 25
                };
            }

            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                author = GetNestedAuthor()
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi',
                      'author': {
                        'name': 'Bob',
                        'age': 25
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelsWithRecursiveProperties()
        {
            var model = new ArticleWithNestedArticles
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Referenced = new ArticleWithNestedArticles
                {
                    Id = "2",
                    Type = "articles",
                    Title = "Another Jsonapi"
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'referenced': {
                        'data': {
                          'id': '2',
                          'type': 'articles'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Another Jsonapi'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCircularReferenceIfItDoesntRecurse()
        {
            var model = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here",
                First = new ModelWithAnotherCircularType
                {
                    Id = "2",
                    Type = "second",
                    Value = "we",
                    Second = new ModelWithCircularType
                    {
                        Id = "3",
                        Type = "first",
                        Value = "go"
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'first',
                    'attributes': {
                      'value': 'here'
                    },
                    'relationships': {
                      'first': {
                        'data': {
                          'id': '2',
                          'type': 'second'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'second',
                      'attributes': {
                        'value': 'we'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '3',
                            'type': 'first'
                          }
                        }
                      }
                    },
                    {
                      'id': '3',
                      'type': 'first',
                      'attributes': {
                        'value': 'go'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeWithCircularReferences()
        {
            var model = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here"
            };

            var another = new ModelWithAnotherCircularType
            {
                Id = "2",
                Type = "second",
                Value = "we",
                Second = model
            };

            model.First = another;

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'first',
                    'attributes': {
                      'value': 'here'
                    },
                    'relationships': {
                      'first': {
                        'data': {
                          'id': '2',
                          'type': 'second'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'second',
                      'attributes': {
                        'value': 'we'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceReferencingItself()
        {
            var model = new ModelReferencingItself
            {
                Id = "1",
                Type = "model",
                Value = "Hi"
            };

            model.Itself = new[]
            {
                model
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'value': 'Hi'
                    },
                    'relationships': {
                      'itself': {
                        'data': [
                          {
                            'id': '1',
                            'type': 'model'
                          }
                        ]
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCircularTypeCollection()
        {
            var model = new ModelWithCircularTypeCollection
            {
                Id = "1",
                Type = "model",
                Value = "Hi",
                First = new ModelWithCircularTypeCollectionNested[]
                {
                    new()
                    {
                        Id = "2",
                        Type = "nested",
                        Value = "Hi again"
                    }
                }
            };

            model.First[0].Second = model;

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'value': 'Hi'
                    },
                    'relationships': {
                      'first': {
                        'data': [
                          {
                            'id': '2',
                            'type': 'nested'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'nested',
                      'attributes': {
                        'value': 'Hi again'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'model'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void Test()
        {
            const string json = @"
{
  'data': {
    'id': '1',
    'type': 'type',
    'attributes': {
      'simple': 'simple',
      'simplevalue': 'simplevalue'
    }
  }
}";

            var m = json.Deserialize<SimpleModel>();
        }

        private class SimpleModel
        {
            public string Id { get; set; }

            public string Type { get; set; }

            public string Simple { get; set; }

            public string SimpleValue { get; set; }
        }
    }
}
