using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeCompoundDocumentTests
    {
        [Fact]
        public void CanSerializeSimpleCompoundDocument()
        {
            var model = new ArticleWithAuthor
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Author = new Author
                {
                    Id = "9",
                    Type = "people",
                    Name = "Dan Gebhardt",
                    Twitter = "dgeb"
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeRelationshipCollection()
        {
            var model = new ArticleWithAuthor
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Comments = new []
                {
                    new Comment
                    {
                        Id = "5",
                        Type = "comments",
                        Body = "first"
                    },
                    new Comment
                    {
                        Id = "12",
                        Type = "comments",
                        Body = "second"
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializBaselineExampleCompoundDocumentWithoutLinks()
        {
            var author = new Author
            {
                Id = "9",
                Type = "people",
                Name = "Dan Gebhardt",
                Twitter = "dgeb"
            };

            var model = new[]
            {
                new ArticleWithAuthor
                {
                    Id = "1",
                    Type = "articles",
                    Title = "JSON:API paints my bikeshed!",
                    Author = author,
                    Comments = new[]
                    {
                        new Comment
                        {
                            Id = "5",
                            Type = "comments",
                            Body = "First!",
                            Author = author
                        },
                        new Comment
                        {
                            Id = "12",
                            Type = "comments",
                            Body = "I like XML better",
                            Author = author
                        }
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'JSON:API paints my bikeshed!'
                    },
                    'relationships': {
                      'author': {
                        'data': { 'type': 'people', 'id': '9' }
                      },
                      'comments': {
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
                    }
                  },
                  {
                    'type': 'comments',
                    'id': '5',
                    'attributes': {
                      'body': 'First!'
                    },
                    'relationships': {
                      'author': {
                        'data': { 'type': 'people', 'id': '9' }
                      }
                    }
                  },
                  {
                    'type': 'comments',
                    'id': '12',
                    'attributes': {
                      'body': 'I like XML better'
                    },
                    'relationships': {
                      'author': {
                        'data': { 'type': 'people', 'id': '9' }
                      }
                    }
                  }]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializBaselineExampleCompoundDocumentWithLinks()
        {
            var author = new Author
            {
                Id = "9",
                Type = "people",
                Name = "Dan Gebhardt",
                Twitter = "dgeb"
            };

            var model = new[]
            {
                new ArticleWithAuthorAndLinks
                {
                    Id = "1",
                    Type = "articles",
                    Title = "JSON:API paints my bikeshed!",
                    Author = new JsonApiRelationship<Author>
                    {
                        Data = author
                    },
                    Comments = new JsonApiRelationship<Comment[]>
                    {
                        Data = new[]
                        {
                            new Comment
                            {
                                Id = "5",
                                Type = "comments",
                                Body = "First!",
                                Author = author
                            },
                            new Comment
                            {
                                Id = "12",
                                Type = "comments",
                                Body = "I like XML better",
                                Author = author
                            }
                        }
                    }
                }
            };

            var json = model.Serialize();

            Assert.Equal(@"
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
                        'data': { 'type': 'people', 'id': '9' }
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
