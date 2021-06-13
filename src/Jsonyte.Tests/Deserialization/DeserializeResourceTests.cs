using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Deserialization
{
    public class DeserializeResourceTests
    {
        [Fact]
        public void CanDeserializeResourceObject()
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

            var article = json.Deserialize<Article>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact]
        public void CanDeserializeResourceWithMeta()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithMeta>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal(10, article.Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeResourceWithTypedMeta()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10,
                      'title': 'Jsonapi'
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithTypedMeta>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal(10, article.Meta.Count);
            Assert.Equal("Jsonapi", article.Meta.Title);
        }

        [Fact]
        public void CanDeserializeNullResourceObject()
        {
            const string json = @"
                {
                  'data': null
                }";

            var article = json.Deserialize<Article>();

            Assert.Null(article);
        }

        [Fact]
        public void CanDeserializeNestedObject()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi',
                      'author': {
                        'name': 'Brown Smith',
                        'title': 'Mr'
                      }
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithNestedAuthor>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.NotNull(article.Author);
            Assert.Equal("Brown Smith", article.Author.Name);
            Assert.Equal("Mr", article.Author.Title);
        }

        [Fact]
        public void CanDeserializeWithPlainDocument()
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

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Data);
            Assert.Single(document.Data);

            Assert.Equal("1", document.Data[0].Id);
            Assert.Equal("articles", document.Data[0].Type);
            Assert.NotNull(document.Data[0].Attributes);
            Assert.Single(document.Data[0].Attributes);
            Assert.Equal("Jsonapi", document.Data[0].Attributes?["title"].GetString());
        }

        [Fact]
        public void CanDeserializeWithDocument()
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

            var article = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(article);
            Assert.NotNull(article.Data);
            Assert.Equal("1", article.Data.Id);
            Assert.Equal("articles", article.Data.Type);
            Assert.Equal("Jsonapi", article.Data.Title);
        }

        [Fact]
        public void ResourceIdMustBeAString()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': 1,
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }";

            var exception = Record.Exception(() => json.Deserialize<ModelWithIntId>());

            Assert.IsType<JsonApiFormatException>(exception);
            Assert.Contains("id must be a string", exception.Message);
        }

        [Fact]
        public void ResourceWithoutTypeThrows()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }";

            var exception = Record.Exception(() => json.Deserialize<Article>());

            Assert.IsType<JsonApiFormatException>(exception);
            Assert.Contains("must contain a 'type' member", exception.Message);
        }

        [Fact]
        public void CanDeserializeResourceWithNoAttributes()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'type': 'article',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }";

            var model = json.Deserialize<ArticleWithNoAttributes>();

            Assert.Equal("1", model.Id);
            Assert.Equal("article", model.Type);
            Assert.Equal("Jsonapi", model.Title);
        }

        [Fact]
        public void CanDeserializeModelWithNullableDecimalAsString()
        {
            const string json = @"
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
                }";

            var options = new JsonSerializerOptions();
            options.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;

            var model = json.Deserialize<ModelWithNullableTypes>(options);

            Assert.Equal("1", model.Id);
            Assert.Equal("model", model.Type);
            Assert.Equal(1, model.ByteValue!.Value);
            Assert.Equal(2, model.SbyteValue!.Value);
            Assert.Equal(3, model.DecimalValue!.Value);
            Assert.Equal(4, model.ShortValue!.Value);
            Assert.Equal(5, model.UshortValue!.Value);
            Assert.Equal(6, model.IntValue!.Value);
            Assert.Equal(7u, model.UintValue!.Value);
            Assert.Equal(8, model.LongValue!.Value);
            Assert.Equal(9u, model.UlongValue!.Value);
            Assert.Equal(10, model.FloatValue!.Value);
            Assert.Equal(11, model.DoubleValue!.Value);

            Assert.IsType<JsonElement>(model.ObjectValue);
            Assert.Equal(JsonValueKind.String, ((JsonElement)model.ObjectValue).ValueKind);
            Assert.Equal("12", ((JsonElement)model.ObjectValue).GetString());
        }

        [Fact]
        public void CanDeserializeModelsWithRecursiveProperties()
        {
            const string json = @"
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
                }";

            var model = json.Deserialize<ArticleWithNestedArticles>();

            Assert.Equal("1", model.Id);
            Assert.Equal("articles", model.Type);
            Assert.Equal("Jsonapi", model.Title);

            Assert.NotNull(model.Referenced);
            Assert.Equal("2", model.Referenced.Id);
            Assert.Equal("articles", model.Referenced.Type);
            Assert.Equal("Another Jsonapi", model.Referenced.Title);
        }

        [Fact]
        public void CanDeserializeWithCircularReferences()
        {
            const string json = @"
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
                }";

            var model = json.Deserialize<ModelWithCircularType>();

            Assert.Equal("1", model.Id);
            Assert.Equal("first", model.Type);
            Assert.Equal("here", model.Value);

            Assert.NotNull(model.First);
            Assert.Equal("2", model.First.Id);
            Assert.Equal("second", model.First.Type);
            Assert.Equal("we", model.First.Value);

            Assert.NotNull(model.First.Second);
            Assert.Same(model, model.First.Second);
        }

        [Fact]
        public void CanDeserializeCircularTypeCollection()
        {
            const string json = @"
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
                }";

            var model = json.Deserialize<ModelWithCircularTypeCollection>();

            Assert.Equal("1", model.Id);
            Assert.Equal("model", model.Type);
            Assert.Equal("Hi", model.Value);

            Assert.NotNull(model.First);
            Assert.Single(model.First);
            Assert.Equal("2", model.First[0].Id);
            Assert.Equal("nested", model.First[0].Type);
            Assert.Equal("Hi again", model.First[0].Value);

            Assert.Same(model, model.First[0].Second);
        }
    }
}
