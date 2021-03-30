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
            Assert.Equal("book", document.Data[0].Attributes?["title"].GetString());
        }

        [Fact]
        public void CanDeserializeResourceObjectWithTypedDocument()
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
            Assert.Equal(10, document.Data[0].Meta?["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeResourceIdentifierWithTypedDocument()
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
        public void CanDeserializeNullResourceObjectWithTypedDocument()
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
        public void CanDeserializeEmptyResourceArrayWithTypedDocument()
        {
            const string json = @"
                {
                  'data': []
                }";

            var document = json.Deserialize<JsonApiDocument<Article[]>>();

            Assert.NotNull(document.Data);
            Assert.Empty(document.Data);
        }

        [Fact]
        public void CanDeserializeLinksWithDocument()
        {
            const string json = @"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'related': 'http://example.com/related'
                  },
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Links);
            Assert.Equal("http://example.com/articles", document.Links.Self?.Href);
            Assert.Equal("http://example.com/related", document.Links.Related?.Href);
        }

        [Fact]
        public void CanDeserializeLinksWithTypedDocument()
        {
            const string json = @"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'related': 'http://example.com/related'
                  },
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document.Links);
            Assert.Equal("http://example.com/articles", document.Links.Self?.Href);
            Assert.Equal("http://example.com/related", document.Links.Related?.Href);
        }

        [Fact]
        public void CanDeserializeIncludedWithDocument()
        {
            const string json = @"
                {
                  'data': null,
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
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Included);
            Assert.NotEmpty(document.Included);
            Assert.Equal(2, document.Included.Length);

            Assert.Equal("9", document.Included[0].Id);
            Assert.Equal("people", document.Included[0].Type);
            Assert.NotNull(document.Included[0].Attributes);
            Assert.Equal("Joe", document.Included[0].Attributes["name"].GetString());

            Assert.Equal("5", document.Included[1].Id);
            Assert.Equal("comments", document.Included[1].Type);
            Assert.NotNull(document.Included[1].Attributes);
            Assert.Equal("first", document.Included[1].Attributes["body"].GetString());
        }

        [Fact]
        public void CanDeserializeIncludedAndRelationshipsWithDocument()
        {
            const string json = @"
                {
                  'data': null,
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
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Included);
            Assert.NotEmpty(document.Included);
            Assert.Equal(2, document.Included.Length);

            Assert.Equal("9", document.Included[0].Id);
            Assert.Equal("people", document.Included[0].Type);
            Assert.NotNull(document.Included[0].Attributes);
            Assert.Equal("Joe", document.Included[0].Attributes["name"].GetString());
            Assert.NotNull(document.Included[0].Relationships?["author"].Data);
            Assert.Single(document.Included[0].Relationships?["author"].Data);
            Assert.Equal("people", document.Included[0].Relationships?["author"].Data[0].Type);
            Assert.Equal("2", document.Included[0].Relationships?["author"].Data[0].Id);
            Assert.Equal("http://example.com/comments/5", document.Included[0].Links?.Self);

            Assert.Equal("5", document.Included[1].Id);
            Assert.Equal("comments", document.Included[1].Type);
            Assert.NotNull(document.Included[1].Attributes);
            Assert.Equal("first", document.Included[1].Attributes["body"].GetString());
            Assert.NotNull(document.Included[1].Relationships?["tags"].Links);
            Assert.Equal("/tags", document.Included[1].Relationships["tags"].Links.Self);
            Assert.NotNull(document.Included[1].Relationships?["tags"].Meta);
            Assert.Equal(5, document.Included[1].Relationships["tags"].Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeIncludedWithTypedDocument()
        {
            const string json = @"
                {
                  'data': null,
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
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document.Included);
            Assert.NotEmpty(document.Included);
            Assert.Equal(2, document.Included.Length);

            Assert.Equal("9", document.Included[0].Id);
            Assert.Equal("people", document.Included[0].Type);
            Assert.NotNull(document.Included[0].Attributes);
            Assert.Equal("Joe", document.Included[0].Attributes["name"].GetString());

            Assert.Equal("5", document.Included[1].Id);
            Assert.Equal("comments", document.Included[1].Type);
            Assert.NotNull(document.Included[1].Attributes);
            Assert.Equal("first", document.Included[1].Attributes["body"].GetString());
        }

        [Fact]
        public void CanDeserializeIncludedAndRelationshipsWithTypedDocument()
        {
            const string json = @"
                {
                  'data': null,
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
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document.Included);
            Assert.NotEmpty(document.Included);
            Assert.Equal(2, document.Included.Length);

            Assert.Equal("9", document.Included[0].Id);
            Assert.Equal("people", document.Included[0].Type);
            Assert.NotNull(document.Included[0].Attributes);
            Assert.Equal("Joe", document.Included[0].Attributes["name"].GetString());
            Assert.NotNull(document.Included[0].Relationships?["author"].Data);
            Assert.Single(document.Included[0].Relationships?["author"].Data);
            Assert.Equal("people", document.Included[0].Relationships?["author"].Data[0].Type);
            Assert.Equal("2", document.Included[0].Relationships?["author"].Data[0].Id);
            Assert.Equal("http://example.com/comments/5", document.Included[0].Links?.Self);

            Assert.Equal("5", document.Included[1].Id);
            Assert.Equal("comments", document.Included[1].Type);
            Assert.NotNull(document.Included[1].Attributes);
            Assert.Equal("first", document.Included[1].Attributes["body"].GetString());
            Assert.NotNull(document.Included[1].Relationships?["tags"].Links);
            Assert.Equal("/tags", document.Included[1].Relationships["tags"].Links.Self);
            Assert.NotNull(document.Included[1].Relationships?["tags"].Meta);
            Assert.Equal(5, document.Included[1].Relationships["tags"].Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeErrorsWithDocument()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Errors);
            Assert.Single(document.Errors);

            Assert.Equal("422", document.Errors[0].Status);
            Assert.Equal("/data/attributes/firstName", document.Errors[0].Source?.Pointer);
            Assert.Equal("Invalid Attribute", document.Errors[0].Title);
            Assert.Equal("First name must contain at least three characters.", document.Errors[0].Detail);
        }

        [Fact]
        public void CanDeserializeErrorsWithTypedDocument()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'status': '422',
                      'source': { 'pointer': '/data/attributes/firstName' },
                      'title':  'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.'
                    }
                  ]
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document.Errors);
            Assert.Single(document.Errors);

            Assert.Equal("422", document.Errors[0].Status);
            Assert.Equal("/data/attributes/firstName", document.Errors[0].Source?.Pointer);
            Assert.Equal("Invalid Attribute", document.Errors[0].Title);
            Assert.Equal("First name must contain at least three characters.", document.Errors[0].Detail);
        }

        [Fact]
        public void CanDeserializeMetaWithDocument()
        {
            const string json = @"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }

        [Fact]
        public void CanDeserializeMetaWithTypedDocument()
        {
            const string json = @"
                {
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }

        [Fact]
        public void CanDeserializeJsonApiObjectWithDocument()
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
        }

        [Fact]
        public void CanDeserializeJsonApiObjectWithTypedDocument()
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }";

            var document = json.Deserialize<JsonApiDocument<Article>>();

            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
        }
    }
}
