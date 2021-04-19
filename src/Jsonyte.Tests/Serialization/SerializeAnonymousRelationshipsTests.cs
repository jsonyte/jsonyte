﻿using System.Linq;
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
    }
}
