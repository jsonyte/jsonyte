using System;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Serialization
{
    public class SerializeJsonApiObjectTests
    {
        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeJsonApiWithSimpleVersion(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = "1.1"
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.1'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeJsonApiWithVersionAndMeta(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = "1.1",
                    Meta = new JsonApiMeta
                    {
                        {"count", 5.ToElement()}
                    }
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.1',
                    'meta': {
                      'count': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void SerializesWithDefaultJsonApiVersion(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject()
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeJsonApiObjectWithNoMembers(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = null
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeJsonApiWithNoVersion(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = null,
                    Meta = new JsonApiMeta
                    {
                        {"count", 5.ToElement()}
                    }
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'meta': {
                      'count': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void JsonApiTypesAreTheSameForBothDocuments()
        {
            var document = new JsonApiDocument();
            var genericDocument = new JsonApiDocument<Article>();

            Assert.Equal(document.JsonApi?.GetType(), genericDocument.JsonApi?.GetType());
        }
    }
}
