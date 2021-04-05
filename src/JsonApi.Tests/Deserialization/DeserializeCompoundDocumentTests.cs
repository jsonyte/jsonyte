using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeCompoundDocumentTests
    {
        [Fact]
        public void CanDeserializeSimpleCompoundDocument()
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
                      'author': {
                        'data': {
                          'type': 'people',
                          'id': '9'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'people',
                      'id': '9',
                      'attributes': {
                        'name': 'Dan Gebhardt',
                        'twitter': 'dgeb'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<ArticleWithAuthor>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Equal("9", article.Author.Id);
            Assert.Equal("people", article.Author.Type);
            Assert.Equal("Dan Gebhardt", article.Author.Name);
            Assert.Equal("dgeb", article.Author.Twitter);
        }

        [Fact]
        public void CanDeserializeRelationshipCollection()
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
                      'comments': {
                        'data': [
                          {
                            'type': 'comments',
                            'id': '5'
                          },
                          {
                            'type': 'comments',
                            'id': '12'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'comments',
                      'id': '5',
                      'attributes': {
                        'body': 'first'
                      }
                    },
                    {
                      'type': 'comments',
                      'id': '12',
                      'attributes': {
                        'body': 'second'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<ArticleWithAuthor>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.NotNull(article.Comments);
            Assert.Equal(2, article.Comments.Length);

            Assert.Equal("5", article.Comments[0].Id);
            Assert.Equal("comments", article.Comments[0].Type);
            Assert.Equal("first", article.Comments[0].Body);

            Assert.Equal("12", article.Comments[1].Id);
            Assert.Equal("comments", article.Comments[1].Type);
            Assert.Equal("second", article.Comments[1].Body);
        }

        [Fact]
        public void CanDeserializeCompoundDocumentWhenIncludedIsFirst()
        {
            const string json = @"
                {
                  'included': [
                    {
                      'type': 'people',
                      'id': '9',
                      'attributes': {
                        'name': 'Dan Gebhardt',
                        'twitter': 'dgeb'
                      }
                    }
                  ],
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'author': {
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
            Assert.Equal("Jsonapi", article.Title);

            Assert.Equal("9", article.Author.Id);
            Assert.Equal("people", article.Author.Type);
            Assert.Equal("Dan Gebhardt", article.Author.Name);
            Assert.Equal("dgeb", article.Author.Twitter);
        }

        [Fact]
        public void CanDeserializeBaselineExampleCompoundDocument()
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'JSON:API paints my bikeshed!'
                    },
                    'links': {
                      'self': 'http://example.com/articles/1'
                    },
                    'relationships': {
                      'author': {
                        'links': {
                          'self': 'http://example.com/articles/1/relationships/author',
                          'related': 'http://example.com/articles/1/author'
                        },
                        'data': { 'type': 'people', 'id': '9' }
                      },
                      'comments': {
                        'links': {
                          'self': 'http://example.com/articles/1/relationships/comments',
                          'related': 'http://example.com/articles/1/comments'
                        },
                        'data': [
                          { 'type': 'comments', 'id': '5' },
                          { 'type': 'comments', 'id': '12' }
                        ]
                      }
                    }
                  }],
                  'included': [{
                    'type': 'people',
                    'id': '9',
                    'attributes': {
                      'name': 'Dan Gebhardt',
                      'twitter': 'dgeb'
                    },
                    'links': {
                      'self': 'http://example.com/people/9'
                    }
                  }, {
                    'type': 'comments',
                    'id': '5',
                    'attributes': {
                      'body': 'First!'
                    },
                    'relationships': {
                      'author': {
                        'data': { 'type': 'people', 'id': '2' }
                      }
                    },
                    'links': {
                      'self': 'http://example.com/comments/5'
                    }
                  }, {
                    'type': 'comments',
                    'id': '12',
                    'attributes': {
                      'body': 'I like XML better'
                    },
                    'relationships': {
                      'author': {
                        'data': { 'type': 'people', 'id': '9' }
                      }
                    },
                    'links': {
                      'self': 'http://example.com/comments/12'
                    }
                  }]
                }";

            var articles = json.Deserialize<ArticleWithAuthor[]>();

            Assert.Single(articles);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("JSON:API paints my bikeshed!", articles[0].Title);

            Assert.NotNull(articles[0].Author);
            Assert.Equal("9", articles[0].Author.Id);
            Assert.Equal("people", articles[0].Author.Type);
            Assert.Equal("Dan Gebhardt", articles[0].Author.Name);
            Assert.Equal("dgeb", articles[0].Author.Twitter);

            Assert.NotNull(articles[0].Comments);
            Assert.Equal(2, articles[0].Comments.Length);

            Assert.Equal("5", articles[0].Comments[0].Id);
            Assert.Equal("comments", articles[0].Comments[0].Type);
            Assert.Equal("First!", articles[0].Comments[0].Body);
            Assert.NotNull(articles[0].Comments[0].Author);
            Assert.NotSame(articles[0].Author, articles[0].Comments[0].Author);
            Assert.Equal("2", articles[0].Comments[0].Author.Id);
            Assert.Equal("people", articles[0].Comments[0].Author.Type);

            Assert.Equal("12", articles[0].Comments[1].Id);
            Assert.Equal("comments", articles[0].Comments[1].Type);
            Assert.Equal("I like XML better", articles[0].Comments[1].Body);
            Assert.NotNull(articles[0].Comments[1].Author);
            Assert.Same(articles[0].Author, articles[0].Comments[1].Author);
        }
    }
}
