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
    }
}
