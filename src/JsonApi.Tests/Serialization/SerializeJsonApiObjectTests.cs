using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeJsonApiObjectTests
    {
        [Fact]
        public void CanSerializeJsonApiWithSimpleVersion()
        {
            var document = new JsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = "1.1"
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.1'
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeJsonApiWithVersionAndMeta()
        {
            var document = new JsonApiDocument
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

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.1',
                    'meta': {
                      'count': 5
                    }
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void SerializesWithDefaultJsonApiVersion()
        {
            var document = new JsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject()
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0'
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeJsonApiObjectWithNoMembers()
        {
            var document = new JsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = null
                }
            };

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeJsonApiWithNoVersion()
        {
            var document = new JsonApiDocument
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

            var json = document.Serialize();

            Assert.Equal(@"
                {
                  'data': null,
                  'jsonapi': {
                    'meta': {
                      'count': 5
                    }
                  }
                }".ToDoubleQuoted(), json, JsonStringEqualityComparer.Default);
        }
    }
}
