using Jsonyte.Tests.Models;
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

        [Fact]
        public void CanParseExistingQueryTyped()
        {
            var builder = new JsonApiUriBuilder<Account>("http://localhost?include=something,second&isTrue")
                .Include(x => x.Balance)
                .Include(x => x.HoldingBank);

            Assert.Equal("?include=something,second,balance,holdingBank&isTrue", builder.Query);
        }
    }
}
