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

            Assert.Equal("?include=something,second", builder.Query);
        }

        [Fact]
        public void CanParseExistingQuery()
        {
            var builder = new JsonApiUriBuilder("http://localhost?include=something,second&isTrue")
                .AddQuery("include", "third");

            Assert.Equal("?include=something,second,third&isTrue", builder.Query);
        }
    }
}
