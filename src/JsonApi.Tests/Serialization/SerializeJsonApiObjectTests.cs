using System;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeJsonApiObjectTests
    {
        [Fact(Skip = "Not implemented")]
        public void CanSerializeSimpleVersion()
        {
            var document = new JsonApiDocument
            {
                Data = null,
                JsonApi = new JsonApiObject
                {
                    Version = new Version(1, 0)
                }
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
    }
}
