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

            var authors = document.Meta?["authors"].EnumerateArray()
                .Select(x => x.GetString())
                .ToArray();

            Assert.NotNull(authors);
            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Example corp", document.Meta?["copyright"].GetString());
            Assert.Equal(15, document.Meta?["loans"].GetInt32());
            Assert.Contains("John Diggs", authors);
            Assert.Contains("Joe Blow", authors);
            Assert.Equal("Book", document.Meta?["details"].GetProperty("title").GetString());
            Assert.Equal("http://example.com", document.Meta?["details"].GetProperty("url").GetString());
            Assert.Equal(2, document.Meta?["details"].GetProperty("count").GetInt32());
        }

        [Fact]
        public void CanDeserializeErrorsWithMeta()
        {
            const string json = @"
                {
                  'errors': [
                    {
                      'status': '422',
                      'title':  'Invalid Attribute'
                    }
                  ],
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }

        [Fact]
        public void CanDeserializeDataWithMeta()
        {
            const string json = @"
                {
                  'data': null,
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }
    }
}
