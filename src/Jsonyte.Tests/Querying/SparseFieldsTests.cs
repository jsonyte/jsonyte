using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Querying
{
    public class SparseFieldsTests
    {
        [Fact]
        public void CanAddField()
        {
            var builder = new JsonApiUriBuilder()
                .IncludeField("articles", "id");

            Assert.Equal("?fields[articles]=id", builder.Query);
        }

        [Fact]
        public void DuplidateFieldIgnored()
        {
            var builder = new JsonApiUriBuilder()
                .IncludeField("articles", "id")
                .IncludeField("articles", "id");

            Assert.Equal("?fields[articles]=id", builder.Query);
        }

        [Fact]
        public void CanAddFieldTyped()
        {
            var builder = new JsonApiUriBuilder<Article>()
                .IncludeField(x => x.Id);

            Assert.Equal("?fields[articles]=id", builder.Query);
        }

        [Fact]
        public void CanAddMultipleFields()
        {
            var builder = new JsonApiUriBuilder()
                .IncludeFields("articles", "id", "author");

            Assert.Equal("?fields[articles]=id,author", builder.Query);
        }

        [Fact]
        public void CanAddMultipleFieldsFromResourcesTyped()
        {
            var builder = new JsonApiUriBuilder()
                .IncludeFields("articles", "id", "author")
                .IncludeFields("authors", "name", "title");

            Assert.Equal("?fields[articles]=id,author&fields[authors]=name,title", builder.Query);
        }

        [Fact]
        public void CanAddMultipleFieldsTyped()
        {
            var builder = new JsonApiUriBuilder<Article>()
                .IncludeField(x => x.Id)
                .IncludeField(x => x.Title);

            Assert.Equal("?fields[articles]=id,title", builder.Query);
        }

        [Fact]
        public void CanAddNestedFieldsTyped()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .IncludeField(x => x.AccountNumber)
                .IncludeField(x => x.HoldingBank.Name)
                .IncludeField(x => x.HoldingBank.Tag);

            Assert.Equal("?fields[accounts]=accountNumber&fields[banks]=name,tag", builder.Query);
        }

        [Fact]
        public void DuplicateFieldsIgnoredTyped()
        {
            var builder = new JsonApiUriBuilder<Account>()
                .IncludeField(x => x.AccountNumber)
                .IncludeField(x => x.AccountNumber)
                .IncludeField(x => x.HoldingBank.Name)
                .IncludeField(x => x.HoldingBank.Name);

            Assert.Equal("?fields[accounts]=accountNumber&fields[banks]=name", builder.Query);
        }

        [Fact]
        public void CanParseExistingFields()
        {
            var builder = new JsonApiUriBuilder("http://localhost?fields[accounts]=accountNumber&fields[banks]=name,tag")
                .IncludeField("accounts", "id")
                .IncludeField("banks", "address")
                .IncludeField("tags", "tag1");

            Assert.Equal("?fields[accounts]=accountNumber,id&fields[banks]=name,tag,address&fields[tags]=tag1", builder.Query);
        }

        [Fact]
        public void CanAddAllFields()
        {
            var builder = new JsonApiUriBuilder<Tag>()
                .IncludeAllFields();

            Assert.Equal("?fields[tags]=id,type,value", builder.Query);
        }
    }
}
