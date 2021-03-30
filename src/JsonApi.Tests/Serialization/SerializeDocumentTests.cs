using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
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
            var document = new JsonApiDocument<Article[]>
            {
                Data = Array.Empty<Article>(),
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
                                        new JsonApiResourceIdentifier
                                        {
                                            Id = "2",
                                            Type = "people"
                                        }
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
            var document = new JsonApiDocument<Article[]>
            {
                Data = Array.Empty<Article>(),
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
                                        new JsonApiResourceIdentifier
                                        {
                                            Id = "2",
                                            Type = "people"
                                        }
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
    }
}
