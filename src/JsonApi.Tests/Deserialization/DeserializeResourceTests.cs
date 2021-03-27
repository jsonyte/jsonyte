using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeResourceTests : ValidationTests
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

            Assert.IsType<JsonApiException>(exception);
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

            Assert.IsType<JsonApiException>(exception);
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
    }
}
