using System;
using System.Collections.Generic;
using System.Linq;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Deserialization
{
    public class DeserializeResourcesTests
    {
        [Fact]
        public void CanDeserializeResourceObjectArray()
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

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.NotEmpty(articles);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("Jsonapi", articles[0].Title);

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal("Jsonapi 2", articles[1].Title);
        }

        [Fact]
        public void CanDeserializeArrayWithPlainDocument()
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

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Data);
            Assert.Equal(2, document.Data.Length);

            Assert.Equal("1", document.Data[0].Id);
            Assert.Equal("articles", document.Data[0].Type);
            Assert.NotNull(document.Data[0].Attributes);
            Assert.Single(document.Data[0].Attributes);
            Assert.Equal("Jsonapi", document.Data[0].Attributes?["title"].GetString());

            Assert.Equal("2", document.Data[1].Id);
            Assert.Equal("articles", document.Data[1].Type);
            Assert.NotNull(document.Data[1].Attributes);
            Assert.Single(document.Data[1].Attributes);
            Assert.Equal("Jsonapi 2", document.Data[1].Attributes?["title"].GetString());
        }

        [Fact]
        public void CanDeserializeArrayWithTypedDocument()
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

            Assert.NotNull(document.Data);
            Assert.Equal(2, document.Data.Length);

            Assert.Equal("1", document.Data[0].Id);
            Assert.Equal("articles", document.Data[0].Type);
            Assert.Equal("Jsonapi", document.Data[0].Title);

            Assert.Equal("2", document.Data[1].Id);
            Assert.Equal("articles", document.Data[1].Type);
            Assert.Equal("Jsonapi 2", document.Data[1].Title);
        }

        [Fact]
        public void CanDeserializeResourcesWithMetaArray()
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10
                    }
                  },
                  {
                    'type': 'articles',
                    'id': '2',
                    'meta': {
                      'count': 5
                    }
                  }]
                }";

            var articles = json.Deserialize<ArticleWithMeta[]>();

            Assert.NotNull(articles);
            Assert.Equal(2, articles.Length);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal(10, articles[0].Meta["count"].GetInt32());

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal(5, articles[1].Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeEmptyResourceArray()
        {
            const string json = @"
                {
                  'data': []
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.Empty(articles);
        }

        [Fact]
        public void CanDeserializeNullResourceArray()
        {
            const string json = @"
                {
                  'data': null
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.Null(articles);
        }

        [Theory]
        [InlineData(typeof(List<Article>))]
        [InlineData(typeof(Article[]))]
        [InlineData(typeof(IList<Article>))]
        [InlineData(typeof(ICollection<Article>))]
        [InlineData(typeof(IEnumerable<Article>))]
        public void CanDeserializeCollections(Type type)
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

            var articles = json.Deserialize(type) as IEnumerable<Article>;

            Assert.NotNull(articles);
            Assert.Equal(2, articles.Count());
        }

        [Fact]
        public void CanDeserializeCollectionWithCircularReferences()
        {
            const string json = @"
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
                }";

            var models = json.Deserialize<ModelWithCircularType[]>();

            Assert.NotNull(models);
            Assert.Equal(2, models.Length);

            Assert.Equal("1", models[0].Id);
            Assert.Equal("first", models[0].Type);
            Assert.Equal("here1", models[0].Value);

            Assert.Equal("2", models[1].Id);
            Assert.Equal("first", models[1].Type);
            Assert.Equal("here2", models[1].Value);

            Assert.NotNull(models[0].First);
            Assert.NotNull(models[1].First);

            Assert.Equal("3", models[0].First.Id);
            Assert.Equal("second", models[0].First.Type);
            Assert.Equal("we1", models[0].First.Value);

            Assert.Equal("4", models[1].First.Id);
            Assert.Equal("second", models[1].First.Type);
            Assert.Equal("we2", models[1].First.Value);

            Assert.NotNull(models[0].First.Second);
            Assert.NotNull(models[1].First.Second);

            Assert.Same(models[0], models[0].First.Second);
            Assert.Same(models[1], models[1].First.Second);
        }

        [Fact]
        public void CanDeserializeResourceObjectWithAttributeArray()
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'model-with-attribute',
                    'id': '1',
                    'attributes': {
                      'value': 'Jsonapi',
                      'intValue': 1
                    }
                  },
                  {
                    'type': 'model-with-attribute',
                    'id': '2',
                    'attributes': {
                      'value': 'Jsonapi 2',
                      'intValue': 2
                    }
                  }]
                }";

            var models = json.Deserialize<ModelWithAttribute[]>();

            Assert.NotNull(models);
            Assert.NotEmpty(models);

            Assert.Equal("1", models[0].Id);
            Assert.Equal("Jsonapi", models[0].Value);
            Assert.Equal(1, models[0].IntValue);

            Assert.Equal("2", models[1].Id);
            Assert.Equal("Jsonapi 2", models[1].Value);
            Assert.Equal(2, models[1].IntValue);
        }

        [Fact]
        public void CanDeserializeResourceObjectWithAttributeArray_MultipleObjects()
        {
            const string json =  @"
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
                }";

            var model = json.Deserialize<ModelWithAttributeAndArray>();

            Assert.NotNull(model);
            Assert.NotEmpty(model.AssociatedObjects);

            Assert.Equal("45", model.Id);
            Assert.Equal("The Title", model.Title);

            Assert.Equal("1", model.AssociatedObjects[0].Id);
            Assert.Equal("First Model", model.AssociatedObjects[0].Value);
            Assert.Equal(12, model.AssociatedObjects[0].IntValue);

            Assert.Equal("12", model.AssociatedObjects[1].Id);
            Assert.Equal("Second Model", model.AssociatedObjects[1].Value);
            Assert.Equal(30, model.AssociatedObjects[1].IntValue);
        }
    }
}
