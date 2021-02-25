using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeDocumentTests
    {
        [Fact]
        public void CanDeserializeResourceObjectWithDocument()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book'
                    }
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document);
            Assert.NotNull(document.Data);
            Assert.Single(document.Data);

            Assert.Equal("articles", document.Data[0].Type);
            Assert.Equal("1", document.Data[0].Id);
            Assert.Equal("book", document.Data[0].Attributes["title"].GetString());
        }

        [Fact]
        public void CanDeserializeResourceObjectWithGenericDocument()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book'
                    }
                  }
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document);
            Assert.NotNull(document.Data);

            Assert.Equal("articles", document.Data.Type);
            Assert.Equal("1", document.Data.Id);
            Assert.Equal("book", document.Data.Title);
        }

        [Fact]
        public void CanDeserializeResourceIdentifierWithDocument()
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

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document);
            Assert.NotNull(document.Data);

            Assert.Equal("articles", document.Data[0].Type);
            Assert.Equal("1", document.Data[0].Id);
            Assert.Equal(10, document.Data[0].Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeResourceIdentifierWithGenericDocument()
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

            var document = json.Deserialize<JsonApiDocument<ArticleWithMeta>>();

            Assert.NotNull(document);
            Assert.NotNull(document.Data);

            Assert.Equal("articles", document.Data.Type);
            Assert.Equal("1", document.Data.Id);
            Assert.Equal(10, document.Data.Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeNullResourceObjectWithDocument()
        {
            const string json = @"
                {
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.Null(document.Data);
        }

        [Fact]
        public void CanDeserializeNullResourceObjectWithGenericDocument()
        {
            const string json = @"
                {
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.Null(document.Data);
        }

        [Fact]
        public void CanDeserializeEmptyResourceArrayWithDocument()
        {
            const string json = @"
                {
                  'data': []
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Data);
            Assert.Empty(document.Data);
        }

        [Fact]
        public void CanDeserializeEmptyResourceArrayWithGenericDocument()
        {
            const string json = @"
                {
                  'data': []
                }";

            var document = json.Deserialize<JsonApiDocument<Article[]>>();

            Assert.NotNull(document.Data);
            Assert.Empty(document.Data);
        }
    }
}
