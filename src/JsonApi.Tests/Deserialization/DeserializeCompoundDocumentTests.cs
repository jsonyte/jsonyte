﻿using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeCompoundDocumentTests
    {
        [Fact(Skip = "Not implemented")]
        public void CanDeserializeCompoundDocument()
        {
            const string json = @"
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
                        'data': { 'type': 'people', 'id': '2' }
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
                }";

            var articles = json.Deserialize<Article[]>();
        }
    }
}
