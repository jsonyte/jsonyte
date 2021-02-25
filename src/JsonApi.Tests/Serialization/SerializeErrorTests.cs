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
    }
}
