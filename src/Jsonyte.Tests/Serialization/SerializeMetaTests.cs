using System;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeMetaTests
    {
        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeOnlyMeta(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Meta = new JsonApiMeta
                {
                    {"name", JsonApiMeta.Value("Bloggs")},
                    {"count", JsonApiMeta.Value(4)},
                    {"authors", JsonApiMeta.Value(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Value(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'count': 4,
                    'authors': [
                      'Tom',
                      'Dick',
                      'Harry'
                    ],
                    'details': {
                      'title': 'book',
                      'active': true,
                      'count': 2
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeMetaWithData(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                Meta = new JsonApiMeta
                {
                    {"name", JsonApiMeta.Value("Bloggs")},
                    {"count", JsonApiMeta.Value(4)},
                    {"authors", JsonApiMeta.Value(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Value(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'count': 4,
                    'authors': [
                      'Tom',
                      'Dick',
                      'Harry'
                    ],
                    'details': {
                      'title': 'book',
                      'active': true,
                      'count': 2
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeMetaWithErrors(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Errors = new[]
                {
                    new JsonApiError
                    {
                        Code = "400",
                        Detail = "No good"
                    }
                },
                Meta = new JsonApiMeta
                {
                    {"name", JsonApiMeta.Value("Bloggs")},
                    {"count", JsonApiMeta.Value(4)},
                    {"authors", JsonApiMeta.Value(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Value(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'code': '400',
                      'detail': 'No good'
                    }
                  ],
                  'meta': {
                    'name': 'Bloggs',
                    'count': 4,
                    'authors': [
                      'Tom',
                      'Dick',
                      'Harry'
                    ],
                    'details': {
                      'title': 'book',
                      'active': true,
                      'count': 2
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaFromAnonymousObject()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                meta = new
                {
                    name = "Joe",
                    count = 4,
                    authors = new[] {"Tom", "Dick"},
                    details = new
                    {
                        title = "book",
                        active = true
                    }
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
                    'meta': {
                      'name': 'Joe',
                      'count': 4,
                      'authors': [
                        'Tom',
                        'Dick'
                      ],
                      'details': {
                        'title': 'book',
                        'active': true
                      }
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void SerializesModelWithMeta()
        {
            var model = new ArticleWithTypedMeta
            {
                Id = "1",
                Type = "articles",
                Meta = new ArticleMeta
                {
                    Count = 4,
                    Title = "Jsonapi"
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'meta': {
                      'count': 4,
                      'title': 'Jsonapi'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
