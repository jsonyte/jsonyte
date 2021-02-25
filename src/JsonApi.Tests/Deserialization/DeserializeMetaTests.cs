using System.Linq;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeMetaTests
    {
        [Fact]
        public void CanDeserializeOnlyMeta()
        {
            const string json = @"
                {
                  'meta': {
                    'copyright': 'Example corp',
                    'loans': 15,
                    'authors': [
                      'John Diggs',
                      'Joe Blow'
                    ],
                    'details': {
                      'title': 'Book',
                      'url': 'http://example.com',
                      'count': 2
                    }
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            var authors = document.Meta["authors"].EnumerateArray()
                .Select(x => x.GetString())
                .ToArray();

            Assert.Null(document.Data);
            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);

            Assert.Equal("Example corp", document.Meta["copyright"].GetString());
            Assert.Equal(15, document.Meta["loans"].GetInt32());
            Assert.Contains("John Diggs", authors);
            Assert.Contains("Joe Blow", authors);
            Assert.Equal("Book", document.Meta["details"].GetProperty("title").GetString());
            Assert.Equal("http://example.com", document.Meta["details"].GetProperty("url").GetString());
            Assert.Equal(2, document.Meta["details"].GetProperty("count").GetInt32());
        }
    }
}
