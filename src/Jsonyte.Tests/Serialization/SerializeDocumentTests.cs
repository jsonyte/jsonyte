using System;
using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class SerializeDocumentTests
    {
        [Fact]
        public void CanSerializeResourceObjectWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = new[]
                {
                    new JsonApiResource
                    {
                        Id = "1",
                        Type = "articles",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"title", "book".ToElement()}
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceObjectWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                Data = new Article
                {
                    Id = "1",
                    Type = "articles",
                    Title = "book"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'book'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceIdentifierWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = new[]
                {
                    new JsonApiResource
                    {
                        Id = "1",
                        Type = "articles",
                        Meta = new JsonApiMeta
                        {
                            {"count", 10.ToElement()}
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceIdentifierWithTypedDocument()
        {
            var document = new JsonApiDocument<ArticleWithMeta>
            {
                Data = new ArticleWithMeta
                {
                    Id = "1",
                    Type = "articles",
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()}
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeNullResourceObjectWithDocument()
        {
            var document = new JsonApiDocument();

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeNullResourceObjectWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>();

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeEmptyResourceArrayWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = Array.Empty<JsonApiResource>()
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': []
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeEmptyResourceArrayWithTypedDocument()
        {
            var document = new JsonApiDocument<Article[]>
            {
                Data = Array.Empty<Article>()
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': []
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeLinksWithDocument()
        {
            var document = new JsonApiDocument
            {
                Links = new JsonApiDocumentLinks
                {
                    Self = "http://example.com/articles",
                    Related = "http://example.com/related"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'related': 'http://example.com/related'
                  },
                  'data': null
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeLinksWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                Links = new JsonApiDocumentLinks
                {
                    Self = "http://example.com/articles",
                    Related = "http://example.com/related"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'related': 'http://example.com/related'
                  },
                  'data': null
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeIncludedWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = Array.Empty<JsonApiResource>(),
                Included = new[]
                {
                    new JsonApiResource
                    {
                        Id = "9",
                        Type = "people",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"name", "Joe".ToElement()}
                        }
                    },
                    new JsonApiResource
                    {
                        Id = "5",
                        Type = "comments",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"body", "first".ToElement()}
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [],
                  'included': [
                    {
                      'type': 'people',
                      'id': '9',
                      'attributes': {
                        'name': 'Joe'
                      }
                    },
                    {
                      'type': 'comments',
                      'id': '5',
                      'attributes': {
                        'body': 'first'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeIncludedWithTypedDocument()
        {
            var article = new ArticleWithAuthor
            {
                Id = "1",
                Type = "article",
                Title = "Cereal-eyes",
                Author = new Author
                {
                    Id = "9",
                    Type = "people",
                    Name = "Joe",
                    Twitter = "joe"
                }
            };

            var document = JsonApiDocument.Create(new[] {article});

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'article',
                      'attributes': {
                        'title': 'Cereal-eyes'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '9',
                            'type': 'people'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '9',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe',
                        'twitter': 'joe'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeIncludedAndRelationshipsWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = Array.Empty<JsonApiResource>(),
                Included = new[]
                {
                    new JsonApiResource
                    {
                        Id = "9",
                        Type = "people",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"name", "Joe".ToElement()}
                        },
                        Relationships = new Dictionary<string, JsonApiRelationship>
                        {
                            {
                                "author", new JsonApiRelationship
                                {
                                    Data = new[]
                                    {
                                        new JsonApiResourceIdentifier("2", "people")
                                    }
                                }
                            }
                        },
                        Links = new JsonApiResourceLinks
                        {
                            Self = "http://example.com/comments/5"
                        }
                    },
                    new JsonApiResource
                    {
                        Id = "5",
                        Type = "comments",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"body", "first".ToElement()}
                        },
                        Relationships = new Dictionary<string, JsonApiRelationship>
                        {
                            {
                                "tags", new JsonApiRelationship
                                {
                                    Links = new JsonApiRelationshipLinks
                                    {
                                        Self = "/tags"
                                    },
                                    Meta = new JsonApiMeta
                                    {
                                        {"count", 5.ToElement()}
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [],
                  'included': [
                    {
                      'type': 'people',
                      'id': '9',
                      'attributes': {
                        'name': 'Joe'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'type': 'people',
                            'id': '2'
                          }
                        }
                      },
                      'links': {
                        'self': 'http://example.com/comments/5'
                      }
                    },
                    {
                      'type': 'comments',
                      'id': '5',
                      'attributes': {
                        'body': 'first'
                      },
                      'relationships': {
                        'tags': {
                          'links': {
                            'self': '/tags'
                          },
                          'meta': {
                            'count': 5
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeIncludedAndRelationshipsWithTypedDocument()
        {
            var author = new Author
            {
                Id = "9",
                Type = "people",
                Name = "Joe",
                Twitter = "joe"
            };

            var article = new ArticleWithAuthor
            {
                Id = "1",
                Type = "articles",
                Title = "Highway or My Way",
                Author = author,
                Comments = new[]
                {
                    new Comment
                    {
                        Id = "5",
                        Type = "comments",
                        Body = "Hi!",
                        Author = author
                    }
                }
            };

            var document = new JsonApiDocument<ArticleWithAuthor[]>
            {
                Data = new[] {article}
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Highway or My Way'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '9',
                            'type': 'people'
                          }
                        },
                        'comments': {
                          'data': [
                            {
                              'id': '5',
                              'type': 'comments'
                            }
                          ]
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '9',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe',
                        'twitter': 'joe'
                      }
                    },
                    {
                      'id': '5',
                      'type': 'comments',
                      'attributes': {
                        'body': 'Hi!'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '9',
                            'type': 'people'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeErrorsWithDocument()
        {
            var document = new JsonApiDocument
            {
                Errors = new[]
                {
                    new JsonApiError
                    {
                        Status = "422",
                        Source = new JsonApiErrorSource
                        {
                            Pointer = "/data/attributes/firstName"
                        },
                        Title = "Invalid Attribute",
                        Detail = "First name must contain at least three characters."
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeErrorsWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                Errors = new[]
                {
                    new JsonApiError
                    {
                        Status = "422",
                        Source = new JsonApiErrorSource
                        {
                            Pointer = "/data/attributes/firstName"
                        },
                        Title = "Invalid Attribute",
                        Detail = "First name must contain at least three characters."
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaWithDocument()
        {
            var document = new JsonApiDocument
            {
                Meta = new JsonApiMeta
                {
                    {"name", "Bloggs".ToElement()},
                    {"active", true.ToElement()}
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMetaWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                Meta = new JsonApiMeta
                {
                    {"name", "Bloggs".ToElement()},
                    {"active", true.ToElement()}
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeJsonApiObjectWithDocument()
        {
            var document = new JsonApiDocument
            {
                JsonApi = new JsonApiObject
                {
                    Version = "1.0"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeJsonApiObjectWithTypedDocument()
        {
            var document = new JsonApiDocument<Article>
            {
                JsonApi = new JsonApiObject
                {
                    Version = "1.0"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCircularRelationshipWithDocument()
        {
            var document = new JsonApiDocument
            {
                Data = new JsonApiResource[]
                {
                    new()
                    {
                        Id = "1",
                        Type = "first",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"value", "here".ToElement()}
                        },
                        Relationships = new Dictionary<string, JsonApiRelationship>
                        {
                            {
                                "first", new JsonApiRelationship
                                {
                                    Data = new JsonApiResourceIdentifier[]
                                    {
                                        new("2", "second")
                                    }
                                }
                            }
                        }
                    }
                },
                Included = new JsonApiResource[]
                {
                    new()
                    {
                        Id = "2",
                        Type = "second",
                        Attributes = new Dictionary<string, JsonElement>
                        {
                            {"value", "we".ToElement()}
                        },
                        Relationships = new Dictionary<string, JsonApiRelationship>
                        {
                            {
                                "second", new JsonApiRelationship
                                {
                                    Data = new JsonApiResourceIdentifier[]
                                    {
                                        new("1", "first")
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'first',
                    'attributes': {
                      'value': 'here'
                    },
                    'relationships': {
                      'first': {
                        'data': {
                          'id': '2',
                          'type': 'second'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'second',
                      'attributes': {
                        'value': 'we'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCircularRelationshipWithTypedDocument()
        {
            var model = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here"
            };

            var another = new ModelWithAnotherCircularType
            {
                Id = "2",
                Type = "second",
                Value = "we",
                Second = model
            };

            model.First = another;

            var document = JsonApiDocument.Create(model);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'first',
                    'attributes': {
                      'value': 'here'
                    },
                    'relationships': {
                      'first': {
                        'data': {
                          'id': '2',
                          'type': 'second'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'second',
                      'attributes': {
                        'value': 'we'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeCollectionWithCircularReferencesWithTypedDocument()
        {
            var model1 = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here1"
            };

            var model2 = new ModelWithCircularType
            {
                Id = "2",
                Type = "first",
                Value = "here2"
            };

            var another1 = new ModelWithAnotherCircularType
            {
                Id = "3",
                Type = "second",
                Value = "we1",
                Second = model1
            };

            var another2 = new ModelWithAnotherCircularType
            {
                Id = "4",
                Type = "second",
                Value = "we2",
                Second = model2
            };

            model1.First = another1;
            model2.First = another2;

            var models = new[] {model1, model2};

            var document = JsonApiDocument.Create(models);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'first',
                      'attributes': {
                        'value': 'here1'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '3',
                            'type': 'second'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'first',
                      'attributes': {
                        'value': 'here2'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '4',
                            'type': 'second'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '3',
                      'type': 'second',
                      'attributes': {
                        'value': 'we1'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    },
                    {
                      'id': '4',
                      'type': 'second',
                      'attributes': {
                        'value': 'we2'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '2',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceWithCircularReferenceFromSameIdAndTypeWithTypedDocument()
        {
            var model = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here",
                First = new ModelWithAnotherCircularType
                {
                    Id = "2",
                    Type = "second",
                    Value = "we",
                    Second = new ModelWithCircularType
                    {
                        Id = "1",
                        Type = "first",
                        Value = "thrown away"
                    }
                }
            };

            var document = JsonApiDocument.Create(model);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'first',
                    'attributes': {
                      'value': 'here'
                    },
                    'relationships': {
                      'first': {
                        'data': {
                          'id': '2',
                          'type': 'second'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '2',
                      'type': 'second',
                      'attributes': {
                        'value': 'we'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeResourceCollectionWithCircularReferenceFromSameIdAndTypeWithTypedDocument()
        {
            var model1 = new ModelWithCircularType
            {
                Id = "1",
                Type = "first",
                Value = "here1",
                First = new ModelWithAnotherCircularType
                {
                    Id = "3",
                    Type = "second",
                    Value = "we1",
                    Second = new ModelWithCircularType
                    {
                        Id = "1",
                        Type = "first",
                        Value = "thrown away"
                    }
                }
            };

            var model2 = new ModelWithCircularType
            {
                Id = "2",
                Type = "first",
                Value = "here2",
                First = new ModelWithAnotherCircularType
                {
                    Id = "4",
                    Type = "second",
                    Value = "we2",
                    Second = new ModelWithCircularType
                    {
                        Id = "2",
                        Type = "first",
                        Value = "thrown away"
                    }
                }
            };

            var models = new[] { model1, model2 };

            var document = JsonApiDocument.Create(models);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'first',
                      'attributes': {
                        'value': 'here1'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '3',
                            'type': 'second'
                          }
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'first',
                      'attributes': {
                        'value': 'here2'
                      },
                      'relationships': {
                        'first': {
                          'data': {
                            'id': '4',
                            'type': 'second'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '3',
                      'type': 'second',
                      'attributes': {
                        'value': 'we1'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '1',
                            'type': 'first'
                          }
                        }
                      }
                    },
                    {
                      'id': '4',
                      'type': 'second',
                      'attributes': {
                        'value': 'we2'
                      },
                      'relationships': {
                        'second': {
                          'data': {
                            'id': '2',
                            'type': 'first'
                          }
                        }
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeDataThatUsesListWithTypedDocument()
        {
            var articles = new List<ArticleWithAuthor>
            {
                new()
                {
                    Id = "1",
                    Type = "article",
                    Title = "Jsonapi",
                    Author = new Author
                    {
                        Id = "2",
                        Type = "author",
                        Name = "Joe"
                    }
                }
            };

            var document = JsonApiDocument.Create(articles);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'article',
                      'attributes': {
                        'title': 'Jsonapi'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '2',
                            'type': 'author'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '2',
                      'type': 'author',
                      'attributes': {
                        'name': 'Joe',
                        'twitter': null
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeDataThatUsesLinkedListWithTypedDocument()
        {
            var articles = new LinkedList<ArticleWithAuthor>();
            articles.AddFirst(new ArticleWithAuthor
            {
                Id = "1",
                Type = "article",
                Title = "Jsonapi",
                Author = new Author
                {
                    Id = "2",
                    Type = "author",
                    Name = "Joe"
                }
            });

            var document = JsonApiDocument.Create(articles);

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'article',
                      'attributes': {
                        'title': 'Jsonapi'
                      },
                      'relationships': {
                        'author': {
                          'data': {
                            'id': '2',
                            'type': 'author'
                          }
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '2',
                      'type': 'author',
                      'attributes': {
                        'name': 'Joe',
                        'twitter': null
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
