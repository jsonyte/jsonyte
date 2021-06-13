using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class SerializeAnonymousRelationshipsTests
    {
        [Fact]
        public void CanSerializeAnonymousRelationshipObject()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                author = new
                {
                    id = "2",
                    type = "people",
                    name = "Bill"
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
        public void CanSerializeAnonymousRelationshipObjectWithDocument()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                author = new
                {
                    id = "2",
                    type = "people",
                    name = "Bill"
                }
            };

            var json = JsonApiDocument.Create(model).Serialize();

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
        public void CanSerializeAnonymousRelationshipCollection()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                authors = new[]
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
                    },
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
                        ]
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
        public void CanSerializeAnonymousRelationshipCollectionWithDocument()
        {
            var model = new
            {
                id = "1",
                type = "articles",
                title = "Jsonapi",
                authors = new[]
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
                    },
                }
            };

            var json = JsonApiDocument.Create(model).Serialize();

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
                        ]
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
        public void CanSerializeExplicitAnonymousRelationshipObject()
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
        public void CanSerializeExplicitAnonymousRelationshipObjectWithDocument()
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
                    }
                }
            };

            var json = JsonApiDocument.Create(model).Serialize();

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
        public void CanSerializeExplicitAnonymousRelationshipCollection()
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
                        },
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
                        ]
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
        public void CanSerializeExplicitAnonymousRelationshipCollectionWithDocument()
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
                        },
                    }
                }
            };

            var json = JsonApiDocument.Create(model).Serialize();

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
                        ]
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
        public void CanSerializeCastedAnonymousRelationshipObject()
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

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthor()
                };
            }

            var model = GetArticle();

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
        public void CanSerializeCastedAnonymousRelationshipObjectWithDocument()
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

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthor()
                };
            }

            var model = GetArticle();

            var json = JsonApiDocument.Create(model).Serialize();

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
        public void CanSerializeCastedAnonymousRelationshipCollection()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthors()
                };
            }

            var model = GetArticle();

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
                        ]
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
        public void CanSerializeCastedAnonymousRelationshipCollectionWithDocument()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthors()
                };
            }

            var model = GetArticle();

            var json = JsonApiDocument.Create(model).Serialize();

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
                        ]
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
        public void CanSerializeCastedExplicitAnonymousRelationshipObject()
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

            object GetAuthorRelationship()
            {
                return new
                {
                    data = GetAuthor()
                };
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthorRelationship()
                };
            }

            var model = GetArticle();

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
        public void CanSerializeCastedExplicitAnonymousRelationshipObjectWithDocument()
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

            object GetAuthorRelationship()
            {
                return new
                {
                    data = GetAuthor()
                };
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthorRelationship()
                };
            }

            var model = GetArticle();

            var json = JsonApiDocument.Create(model).Serialize();

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
        public void CanSerializeCastedExplicitAnonymousRelationshipCollection()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetAuthorsRelationship()
            {
                return new
                {
                    data = GetAuthors()
                };
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthorsRelationship()
                };
            }

            var model = GetArticle();

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
                        ]
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
        public void CanSerializeCastedExplicitAnonymousRelationshipCollectionWithDocument()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetAuthorsRelationship()
            {
                return new
                {
                    data = GetAuthors()
                };
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthorsRelationship()
                };
            }

            var model = GetArticle();

            var json = JsonApiDocument.Create(model).Serialize();

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
                        ]
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
        public void CanSerializeCastedAnonymousRelationshipCollectionWithCastedNonRelationshipCollection()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetTags()
            {
                return new[] {"tag1", "tag2"}.Select(x => x);
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthors(),
                    tags = GetTags()
                };
            }

            var model = GetArticle();

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi',
                      'tags': ['tag1', 'tag2']
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
                        ]
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
        public void CanSerializeCastedExplicitRelationshipsWhenTheContentsChanges()
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

            object GetEmptyAuthor()
            {
                return null;
            }

            object GetAuthorRelationshipEmpty()
            {
                return new
                {
                    data = GetEmptyAuthor()
                };
            }

            object GetArticleWithNoAuthor()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthorRelationshipEmpty()
                };
            }

            object GetArticleWithAuthor()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = GetAuthor()
                };
            }

            var options = new JsonSerializerOptions();

            GetArticleWithNoAuthor().Serialize(options);

            var json = GetArticleWithAuthor().Serialize(options);

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
        public void CanSerializeCollectionWithCircularRelationships()
        {
            object GetNested(bool value)
            {
                return value
                    ? new
                    {
                        id = "1",
                        type = "articles"
                    }
                    : null;
            }

            IEnumerable<object> GetModel()
            {
                return new[]
                {
                    new
                    {
                        id = "1",
                        type = "articles",
                        title = "Jsonapi1",
                        history = GetNested(false)
                    },
                    new
                    {
                        id = "2",
                        type = "articles",
                        title = "Jsonapi2",
                        history = GetNested(true)
                    }
                };
            }

            var model = GetModel();

            var document = JsonApiDocument.Create(model);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi1',
                        'history': null
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi2'
                      },
                      'relationships': {
                        'history': {
                          'data': {
                            'id': '1',
                            'type': 'articles'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceWithCircularRelationship()
        {
            object GetModel()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    author = new
                    {
                        id = "2",
                        type = "authors",
                        name = "Joe",
                        location = new
                        {
                            id = "3",
                            type = "locations",
                            article = new
                            {
                                id = "1",
                                type = "articles",
                                title = "thrown away"
                            }
                        }
                    }
                };
            }

            var model = GetModel();

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'author': {
                        'data': {
                          'id': '2',
                          'type': 'authors'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'authors',
                      'attributes': {
                        'name': 'Joe'
                      },
                      'relationships': {
                        'location': {
                          'data': {
                            'id': '3',
                            'type': 'locations'
                          }
                        }
                      }
                    },
                    {
                      'id': '3',
                      'type': 'locations',
                      'relationships': {
                        'article': {
                          'data': {
                            'id': '1',
                            'type': 'articles'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceCollectionWithCircularRelationshipsOfSameIdAndType()
        {
            object GetModel()
            {
                return new[]
                {
                    new
                    {
                        id = "1",
                        type = "articles",
                        value = "Jsonapi1",
                        authors = new[]
                        {
                            new
                            {
                                id = "1",
                                type = "authors",
                                name = "Joe",
                                related = new
                                {
                                    id = "2",
                                    type = "articles"
                                }
                            },
                            new
                            {
                                id = "2",
                                type = "authors",
                                name = "Blow",
                                related = new
                                {
                                    id = "2",
                                    type = "articles"
                                }
                            }
                        }
                    },
                    new
                    {
                        id = "2",
                        type = "articles",
                        value = "Jsonapi2",
                        authors = new[]
                        {
                            new
                            {
                                id = "1",
                                type = "authors",
                                name = "Joe",
                                related = new
                                {
                                    id = "2",
                                    type = "articles"
                                }
                            }
                        }
                    }
                };
            }

            var document = JsonApiDocument.Create(GetModel());

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'value': 'Jsonapi1'
                      },
                      'relationships': {
                        'authors': {
                          'data': [
                            {
                              'id': '1',
                              'type': 'authors'
                            },
                            {
                              'id': '2',
                              'type': 'authors'
                            }
                          ]
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'value': 'Jsonapi2'
                      },
                      'relationships': {
                        'authors': {
                          'data': [
                            {
                              'id': '1',
                              'type': 'authors'
                            }
                          ]
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '1',
                      'type': 'authors',
                      'attributes': {
                        'name': 'Joe'
                      },
                      'relationships': {
                        'related': {
                          'data': {
                            'id': '2',
                            'type': 'articles'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'authors',
                      'attributes': {
                        'name': 'Blow'
                      },
                      'relationships': {
                        'related': {
                          'data': {
                            'id': '2',
                            'type': 'articles'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeRelationshipFromQueryable()
        {
            IEnumerable<object> GetData()
            {
                return new[]
                {
                    new
                    {
                        id = "3",
                        type = "books",
                        value = "Up is down"
                    },
                    new
                    {
                        id = "4",
                        type = "books",
                        value = "Right is left"
                    }
                };
            }

            IQueryable<object> GetRelationship()
            {
                return GetData().AsQueryable();
            }

            var model = new
            {
                id = "1",
                type = "model",
                name = "weird",
                books = GetRelationship()
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'name': 'weird'
                    },
                    'relationships': {
                      'books': {
                        'data': [
                          {
                            'id': '3',
                            'type': 'books'
                          },
                          {
                            'id': '4',
                            'type': 'books'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '3',
                      'type': 'books',
                      'attributes': {
                        'value': 'Up is down'
                      }
                    },
                    {
                      'id': '4',
                      'type': 'books',
                      'attributes': {
                        'value': 'Right is left'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeAnonymousCollectionThatIsNotRelationship()
        {
            IEnumerable<object> GetData()
            {
                return new[]
                {
                    new
                    {
                        name = "Bob",
                        age = 25
                    },
                    new
                    {
                        name = "Jane",
                        age = 28
                    }
                };
            }

            var model = new
            {
                id = "1",
                type = "model",
                title = "Jsonapi",
                employees = GetData()
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': 'Jsonapi',
                      'employees': [
                        {
                          'name': 'Bob',
                          'age': 25
                        },
                        {
                          'name': 'Jane',
                          'age': 28
                        }
                      ]
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
