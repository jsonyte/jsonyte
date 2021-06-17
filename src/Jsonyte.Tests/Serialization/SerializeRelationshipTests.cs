using System.Collections.Generic;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class SerializeRelationshipTests
    {
        [Fact]
        public void CanSerializeRelationshipWithDataAsAttributeName()
        {
            var model = new ArticleWithTagsData
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Tags = new List<TagWithData>
                {
                    new()
                    {
                        Id = "1",
                        Type = "tags",
                        Value = "tagname",
                        Data = "data"
                    }
                }
            };

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
                      'tags': {
                        'data': [
                          {
                            'id': '1',
                            'type': 'tags'
                          }
                        ]
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '1',
                      'type': 'tags',
                      'attributes': {
                        'value': 'tagname',
                        'data': 'data'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeExplicitRelationshipResource()
        {
            var model = new ArticleWithExplicitRelationship
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Author = new JsonApiRelationship<Author>
                {
                    Data = new Author
                    {
                        Id = "4",
                        Type = "people",
                        Name = "Joe Blow",
                        Twitter = "jblow"
                    }
                }
            };

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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeExplicitRelationshipArrayResource()
        {
            var model = new ArticleWithExplicitRelationshipArray
            {
                Id = "1",
                Type = "articles",
                Title = "Jsonapi",
                Author = new JsonApiRelationship<Author[]>
                {
                    Data = new Author[]
                    {
                        new()
                        {
                            Id = "4",
                            Type = "people",
                            Name = "Joe Blow",
                            Twitter = "jblow"
                        }
                    }
                }
            };

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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
