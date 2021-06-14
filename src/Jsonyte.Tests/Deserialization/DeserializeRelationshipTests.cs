using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Deserialization
{
    public class DeserializeRelationshipTests
    {
        [Fact]
        public void CanDeserializeSingleRelationship()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Rails is Omakase'
                    },
                    'relationships': {
                      'author': {
                        'links': {
                          'self': '/articles/1/relationships/author',
                          'related': '/articles/1/author'
                        },
                        'data': {
                          'type': 'people',
                          'id': '9'
                        }
                      }
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithAuthor>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Rails is Omakase", article.Title);

            Assert.Equal("people", article.Author.Type);
            Assert.Equal("9", article.Author.Id);
        }

        [Fact]
        public void CanDeserializeRelationshipCollectionWithNoInclude()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'tags': {
                        'data': [
                          {
                            'type': 'tags',
                            'id': '4'
                          }
                        ]
                      }
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithTags>();

            Assert.NotNull(article);
            Assert.NotNull(article.Tags);
            Assert.NotEmpty(article.Tags);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Single(article.Tags);
            Assert.Equal("4", article.Tags[0].Id);
            Assert.Equal("tags", article.Tags[0].Type);
            Assert.Null(article.Tags[0].Value);
        }

        [Fact]
        public void CanDeserializeRelationshipThatContainsDataKeyword()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'tags': {
                        'data': [
                          {
                            'type': 'tags',
                            'id': '4'
                          }
                        ]
                      }
                    }
                  }
                }";

            var article = json.Deserialize<ArticleWithTagsData>();

            Assert.NotNull(article);
            Assert.NotNull(article.Tags);
            Assert.NotEmpty(article.Tags);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Single(article.Tags);
            Assert.Equal("4", article.Tags[0].Id);
            Assert.Equal("tags", article.Tags[0].Type);
            Assert.Null(article.Tags[0].Value);
        }
    }
}
