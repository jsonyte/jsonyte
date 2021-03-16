using System;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeErrorTests
    {
        [Fact]
        public void CanSerializeSingleErrorAsArray()
        {
            var errors = new[]
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
            };

            var json = errors.Serialize();

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'status': '422',
                      'title': 'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.',
                      'source': {
                        'pointer': '/data/attributes/firstName'
                      }
                    }
                  ]
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializerSingleErrorAsObject()
        {
            var error = new JsonApiError
            {
                Status = "422",
                Source = new JsonApiErrorSource
                {
                    Pointer = "/data/attributes/firstName"
                },
                Title = "Invalid Attribute",
                Detail = "First name must contain at least three characters."
            };

            var json = error.Serialize();

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'status': '422',
                      'title': 'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.',
                      'source': {
                        'pointer': '/data/attributes/firstName'
                      }
                    }
                  ]
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeMultipleErrorsAsArray()
        {
            var errors = new[]
            {
                new JsonApiError
                {
                    Id = "1",
                    Links = new JsonApiErrorLinks
                    {
                        {"next", new JsonApiLink {Href = "http://next"}},
                        {"about", new JsonApiLink {Href = "http://about"}},
                    },
                    Status = "422",
                    Code = "Invalid",
                    Title = "Invalid Attribute",
                    Detail = "First name must contain at least three characters.",
                    Source = new JsonApiErrorSource
                    {
                        Pointer = "/data/attributes/firstName"
                    },
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()},
                        {"name", "first".ToElement()}
                    }
                },
                new JsonApiError
                {
                    Id = "2",
                    Links = new JsonApiErrorLinks
                    {
                        {"next", new JsonApiLink {Href = "http://next2"}},
                        {"about", new JsonApiLink {Href = "http://about2"}},
                    },
                    Status = "522",
                    Code = "Inconceivable",
                    Title = "Inconceivable Attribute",
                    Detail = "Princess bride must be watched.",
                    Source = new JsonApiErrorSource
                    {
                        Pointer = "/data/attributes/princess"
                    },
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()}
                    }
                }
            };

            var json = errors.Serialize();

            Assert.Equal(@"
                {
                  'errors': [
                    {
                      'id': '1',
                      'links': {
                        'next': 'http://next',
                        'about': 'http://about'
                      },
                      'status': '422',
                      'code': 'Invalid',
                      'title': 'Invalid Attribute',
                      'detail': 'First name must contain at least three characters.',
                      'source': {
                        'pointer': '/data/attributes/firstName'
                      },
                      'meta': {
                        'count': 10,
                        'name': 'first'
                      }
                    },
                    {
                      'id': '2',
                      'links': {
                        'next': 'http://next2',
                        'about': 'http://about2'
                      },
                      'status': '522',
                      'code': 'Inconceivable',
                      'title': 'Inconceivable Attribute',
                      'detail': 'Princess bride must be watched.',
                      'source': {
                        'pointer': '/data/attributes/princess'
                      },
                      'meta': {
                        'count': 10
                      }
                    }
                  ]
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeEmptyErrors()
        {
            var errors = Array.Empty<JsonApiError>();

            var json = errors.Serialize();

            Assert.Equal(@"
                {
                  'errors': []
                }
                ".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }
    }
}
