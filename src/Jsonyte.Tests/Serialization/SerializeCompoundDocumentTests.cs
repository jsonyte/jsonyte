using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
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

        [Fact(Skip = "Not yet implemented")]
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

        [Fact]
        public void CanSerializeModelWithExplicitResourceAndLinks()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                author = new
                {
                    data = new
                    {
                        id = "2",
                        type = "people",
                        name = "Bill"
                    },
                    links = new
                    {
                        self = "http://me"
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
                      'author': {
                        'data': {
                          'type': 'people',
                          'id': '2'
                        },
                        'links': {
                          'self': 'http://me'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'people',
                      'id': '2',
                      'attributes': {
                        'name': 'Bill'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitResourceArrayAndLinks()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                authors = new
                {
                    data = new[]
                    {
                        new
                        {
                            id = "2",
                            type = "people",
                            name = "Bill"
                        },
                        new
                        {
                            id = "3",
                            type = "people",
                            name = "Ted"
                        }
                    },
                    links = new
                    {
                        self = "http://me"
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
                      'authors': {
                        'data': [
                          {
                            'type': 'people',
                            'id': '2'
                          },
                          {
                            'type': 'people',
                            'id': '3'
                          }
                        ],
                        'links': {
                          'self': 'http://me'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'people',
                      'id': '2',
                      'attributes': {
                        'name': 'Bill'
                      }
                    },
                    {
                      'type': 'people',
                      'id': '3',
                      'attributes': {
                        'name': 'Ted'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithRelationshipAsAnonymousObject()
        {
            object GetAuthor()
            {
                return new
                {
                    id = "2",
                    type = "people",
                    name = "Bill"
                };
            }

            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                author = GetAuthor()
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
                          'id': '2'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'people',
                      'id': '2',
                      'attributes': {
                        'name': 'Bill'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeModelWithExplicitResourceArrayAsObject()
        {
            object GetAuthors()
            {
                return new
                {
                    data = new[]
                    {
                        new
                        {
                            id = "2",
                            type = "people",
                            name = "Bill"
                        },
                        new
                        {
                            id = "3",
                            type = "people",
                            name = "Ted"
                        }
                    },
                    links = new
                    {
                        self = "http://me"
                    }
                };
            }

            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                authors = GetAuthors()
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
                      'authors': {
                        'data': [
                          {
                            'type': 'people',
                            'id': '2'
                          },
                          {
                            'type': 'people',
                            'id': '3'
                          }
                        ],
                        'links': {
                          'self': 'http://me'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'type': 'people',
                      'id': '2',
                      'attributes': {
                        'name': 'Bill'
                      }
                    },
                    {
                      'type': 'people',
                      'id': '3',
                      'attributes': {
                        'name': 'Ted'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
