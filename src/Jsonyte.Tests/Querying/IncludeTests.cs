using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Querying
{
    public class IncludeTests
    {
        [Fact]
        public void CanIncludeRelationship()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .Include(x => x.HoldingBank);

            Assert.Equal("?include=holdingBank", builder.Query);
        }

        [Fact]
        public void CanIncludeRelationshipWithName()
        {
            var builder = new JsonApiUriBuilder()
                .Include("holdingBank");

            Assert.Equal("?include=holdingBank", builder.Query);
        }

        [Fact]
        public void CanIncludeMultipleRelationships()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .Include(x => x.HoldingBank)
                .Include(x => x.Transactions);

            Assert.Equal("?include=holdingBank,transactions".EncodeQuery(), builder.Query);
        }

        [Fact]
        public void CanIncludeNestedRelationship()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .Include(x => x.HoldingBank.Tag);

            Assert.Equal("?include=holdingBank.tag".EncodeQuery(), builder.Query);
        }

        [Fact]
        public void IncludeUsesPropertyNameAttribute()
        {
            var builder = new JsonApiUriBuilder<AccountWithAttributes>()
                .Include(x => x.Bank);

            Assert.Equal("?include=myBank".EncodeQuery(), builder.Query);
        }
    }
}
