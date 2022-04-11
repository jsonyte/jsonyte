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
        public void CanDeserializeSingleRelationshipWithMeta()
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
                          'id': '9',
                          'meta': {
                            'level': 15
                          }
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
        public void CanDeserializeRelationshipCollectionWithMeta()
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
                            'id': '4',
                            'meta': {
                              'level': 15
                            }
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

        [Fact]
        public void CanDeserializeModelWithTypedRelationship()
        {
            const string json = @"
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
                          'id': '4',
                          'type': 'people'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<ArticleWithExplicitRelationship>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);
            Assert.NotNull(article.Author.Data);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Equal("4", article.Author.Data.Id);
            Assert.Equal("people", article.Author.Data.Type);
            Assert.Equal("Joe Blow", article.Author.Data.Name);
            Assert.Equal("jblow", article.Author.Data.Twitter);
        }

        [Fact]
        public void CanSerializeModelWithTypedRelationshipAndMeta()
        {
            const string json = @"
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
                          'id': '4',
                          'type': 'people'
                        },
                        'links': {
                          'first': 'http://here'
                        },
                        'meta': {
                          'count': 5
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<ArticleWithExplicitRelationship>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);
            Assert.NotNull(article.Author.Data);
            Assert.NotNull(article.Author.Links);
            Assert.NotNull(article.Author.Meta);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Equal("4", article.Author.Data.Id);
            Assert.Equal("people", article.Author.Data.Type);
            Assert.Equal("Joe Blow", article.Author.Data.Name);
            Assert.Equal("jblow", article.Author.Data.Twitter);

            Assert.Equal("http://here", article.Author.Links.First?.Href);
            Assert.Equal(5, article.Author.Meta["count"].GetInt32());
        }

        [Fact]
        public void CanDeserializeModelWithTypedRelationshipArray()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'type': 'articles',
                    'attributes': {
                      'title': 'Jsonapi'
                    },
                    'relationships': {
                      'author': {
                        'data': [
                          {
                            'id': '4',
                            'type': 'people'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<ArticleWithExplicitRelationshipArray>();

            Assert.NotNull(article);
            Assert.NotNull(article.Author);
            Assert.NotNull(article.Author.Data);

            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);

            Assert.Single(article.Author.Data);
            Assert.Equal("4", article.Author.Data[0].Id);
            Assert.Equal("people", article.Author.Data[0].Type);
            Assert.Equal("Joe Blow", article.Author.Data[0].Name);
            Assert.Equal("jblow", article.Author.Data[0].Twitter);
        }

        [Fact]
        public void CanDeserializeResourceWithSkippedRelationship()
        {
            const string json = @"
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
                          'id': '4',
                          'type': 'people'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<Article>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact]
        public void CanDeserializeResourceWithSkippedRelationshipAndMeta()
        {
            const string json = @"
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
                          'id': '4',
                          'type': 'people',
                          'meta': {
                            'level': 15
                          }
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var article = json.Deserialize<Article>();

            Assert.NotNull(article);
            Assert.Equal("1", article.Id);
            Assert.Equal("articles", article.Type);
            Assert.Equal("Jsonapi", article.Title);
        }

        [Fact]
        public void CanDeserializeResourceCollectionWithSkippedRelationship()
        {
            const string json = @"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi'
                      },
                      'relationships': {
                        'author': {
                          'data': [
                            {
                              'id': '4',
                              'type': 'people'
                            }
                          ]
                        }
                      }
                    },
                    {
                      'id': '2',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi 2'
                      },
                      'relationships': {
                        'author': {
                          'data': [
                            {
                              'id': '4',
                              'type': 'people'
                            }
                          ]
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.Equal(2, articles.Length);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("Jsonapi", articles[0].Title);

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal("Jsonapi 2", articles[1].Title);
        }

        [Fact]
        public void CanDeserializeResourceCollectionWithSkippedRelationshipAndMeta()
        {
            const string json = @"
                {
                  'data': [
                    {
                      'id': '1',
                      'type': 'articles',
                      'attributes': {
                        'title': 'Jsonapi'
                      },
                      'relationships': {
                        'author': {
                          'data': [
                            {
                              'id': '4',
                              'type': 'people',
                              'meta': {
                                'level': 15
                              }
                            }
                          ]
                        }
                      }
                    }
                  ],
                  'included': [
                    {
                      'id': '4',
                      'type': 'people',
                      'attributes': {
                        'name': 'Joe Blow',
                        'twitter': 'jblow'
                      }
                    }
                  ]
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.Single(articles);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("Jsonapi", articles[0].Title);
        }
    }
}
