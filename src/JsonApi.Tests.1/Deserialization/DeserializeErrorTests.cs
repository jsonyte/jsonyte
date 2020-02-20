using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeErrorTests
    {
        [Fact]
        public void CanConvertSingleErrorAsArray()
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

            var errors = json.Deserialize<JsonApiError[]>();

            Assert.Single(errors);
            Assert.Equal("422", errors.First().Status);
            Assert.Equal("Invalid Attribute", errors.First().Title);
            Assert.Equal("First name must contain at least three characters.", errors.First().Detail);
            Assert.NotNull(errors.First().Source);
            Assert.Equal("/data/attributes/firstName", errors.First().Source.Pointer.ToString());
        }

        [Fact]
        public void CanConvertMetaOnlyErrors()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'meta': {
                        'copyright': 'jsonapi',
                        'authors': [
                          'Bob Jane',
                          'James Bond'
                        ]
                      }
                    }
                  ]
                }";

            var errors = json.Deserialize<JsonApiError[]>();

            Assert.NotEmpty(errors);
            Assert.NotEmpty(errors.First().Meta);
        }

        [Fact]
        public void CanConvertErrorsWithDocumentMeta()
        {
            const string json = @"
                {
                  'meta': {
                    'name': 'Dave',
                    'nemesis': [
                      'HAL',
                      'Space'
                    ]
                  },
                  'errors': [
                    {
                      'title': 'No permission',
                      'meta': {
                        'copyright': 'jsonapi',
                        'authors': [
                          'Bob Jane',
                          'James Bond'
                        ]
                      }
                    }
                  ]
                }";

            var errors = json.Deserialize<JsonApiError[]>();

            Assert.NotEmpty(errors);
            Assert.NotEmpty(errors.First().Meta);
            Assert.Equal("No permission", errors.First().Title);
        }

        [Theory]
        [InlineData(typeof(List<JsonApiError>))]
        [InlineData(typeof(JsonApiError[]))]
        [InlineData(typeof(IList<JsonApiError>))]
        [InlineData(typeof(ICollection<JsonApiError>))]
        [InlineData(typeof(IEnumerable<JsonApiError>))]
        public void CanConvertMultipleErrorsAsCollections(Type type)
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'id': '1',
                      'links': {
                        'about': 'http://example.com'
                      },
                      'status': '404',
                      'code': '123',
                      'title': 'Value is too short',
                      'detail': 'First name must contain at least three characters.',
                      'source': {
                        'pointer': '/data/attributes/firstName',
                        'parameter': 'id'
                      },
                      'meta': {
                        'copyright': 'jsonapi',
                        'authors': [
                          'Bob Jane',
                          'James Bond'
                        ]
                      }
                    },
                    {
                      'id': '2',
                      'links': {
                        'about': {
                          'href': 'http://example.com',
                          'meta': {
                            'count': 10,
                            'messages': [
                              'error 1',
                              'error 2'
                            ]
                          }
                        }
                      },
                      'status': '501',
                      'code': '456',
                      'title': 'No permission',
                      'detail': 'No permission to access the first name',
                      'source': {
                        'pointer': '/data/attributes/firstName',
                        'parameter': 'id'
                      },
                      'meta': {
                        'copyright': 'jsonapi',
                        'authors': [
                          'Bob Jane',
                          'James Bond'
                        ]
                      }
                    },
                    {
                      'code': '226',
                      'source': { 'pointer': '' },
                      'title': 'Password and password confirmation do not match.'
                    }
                  ]
                }";

            var collection = json.Deserialize(type);
            var enumerable = collection as IEnumerable<JsonApiError>;

            Assert.NotNull(collection);
            Assert.Equal(3, enumerable?.Count());
        }

        [Fact]
        public void SingleResourceDocumentWithNoErrorsReturnsNull()
        {
            const string json = @"
            {
              'data': {
                'type': 'articles',
                'id': '1',
                'attributes': {
                  'title': 'json'
                }
              }
            }";

            var errors = json.Deserialize<JsonApiError[]>();

            Assert.Null(errors);
        }

        [Fact]
        public void MultipleResourceDocumentWithNoErrorsReturnsNull()
        {
            const string json = @"
            {
              'data': [
                {
                  'type': 'articles',
                  'id': '1',
                  'attributes': {
                    'title': 'json'
                  }
                },
                {
                  'type': 'articles',
                  'id': '2',
                  'attributes': {
                    'title': 'api'
                  }
                }
              ]
            }";

            var errors = json.Deserialize<JsonApiError[]>();

            Assert.Null(errors);
        }

        [Fact]
        public void DocumentWithErrorsAndDataShouldThrow()
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
                  ],
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'json'
                    }
                  }
                }";

            var exception = Record.Exception(() => json.Deserialize<JsonApiError[]>());

            Assert.IsType<JsonApiException>(exception);
            Assert.Contains("must not contain both 'data' and 'errors'", exception.Message);
        }
    }
}
