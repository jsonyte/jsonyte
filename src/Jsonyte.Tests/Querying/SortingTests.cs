using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Querying
{
    public class SortingTests
    {
        [Fact]
        public void CanAddSorting()
        {
            var builder = new JsonApiUriBuilder()
                .OrderBy("balance")
                .OrderBy("amount")
                .OrderByDescending("id");

            Assert.Equal("?sort=balance,amount,-id", builder.Query);
        }

        [Fact]
        public void CanAddSortingByRelationship()
        {
            var builder = new JsonApiUriBuilder()
                .OrderBy("bank.balance")
                .OrderBy("account.amount")
                .OrderByDescending("account.id");

            Assert.Equal("?sort=bank.balance,account.amount,-account.id", builder.Query);
        }

        [Fact]
        public void CanAddSortingTyped()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .OrderBy(x => x.Balance)
                .OrderBy(x => x.AccountNumber)
                .OrderByDescending(x => x.HoldingBank);

            Assert.Equal("?sort=balance,accountNumber,-holdingBank", builder.Query);
        }

        [Fact]
        public void DuplicateSortsIgnored()
        {
            var builder = new JsonApiUriBuilder()
                .OrderBy("balance")
                .OrderBy("balance")
                .OrderByDescending("amount")
                .OrderByDescending("amount");

            Assert.Equal("?sort=balance,-amount", builder.Query);
        }

        [Fact]
        public void DuplicateSortsIgnoredTyped()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .OrderBy(x => x.Balance)
                .OrderBy(x => x.Balance)
                .OrderByDescending(x => x.Id)
                .OrderByDescending(x => x.Id);

            Assert.Equal("?sort=balance,-id", builder.Query);
        }
    }
}
