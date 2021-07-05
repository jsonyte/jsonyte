using Xunit;

namespace Jsonyte.Tests.Querying
{
    public class ParsingTests
    {
        [Fact]
        public void CanAddQuery()
        {
            var builder = new JsonApiUriBuilder()
                .AddQuery("include", "something")
                .AddQuery("isTrue");

            Assert.Equal("?include=something&isTrue", builder.Query);
        }

        [Fact]
        public void CanAddMultipleQueryValues()
        {
            var builder = new JsonApiUriBuilder()
                .AddQuery("include", "something", "second");

            Assert.Equal("?include=something&include=second", builder.Query);
        }
    }
}
