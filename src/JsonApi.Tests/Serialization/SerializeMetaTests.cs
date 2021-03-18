using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeMetaTests
    {
        [Fact]
        public void CanSerializeOnlyMeta()
        {
            var document = new JsonApiDocument
            {
                Meta = new JsonApiMeta
                {
                    {"name", JsonApiMeta.Create("Bloggs")},
                    {"count", JsonApiMeta.Create(4)},
                    {"authors", JsonApiMeta.Create(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Create(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.Serialize();

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
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaWithData()
        {
            var document = new JsonApiDocument
            {
                Data = null,
                Meta = new JsonApiMeta
                {
                    {"name", JsonApiMeta.Create("Bloggs")},
                    {"count", JsonApiMeta.Create(4)},
                    {"authors", JsonApiMeta.Create(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Create(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.Serialize();

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
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaWithErrors()
        {
            var document = new JsonApiDocument
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
                    {"name", JsonApiMeta.Create("Bloggs")},
                    {"count", JsonApiMeta.Create(4)},
                    {"authors", JsonApiMeta.Create(new[] {"Tom", "Dick", "Harry"})},
                    {"details", JsonApiMeta.Create(new {title = "book", active = true, count = 2})}
                }
            };

            var json = document.Serialize();

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
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }
    }
}
