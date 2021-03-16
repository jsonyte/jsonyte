using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeLinksTests
    {
        [Fact(Skip = "Not implemented")]
        public void JsonMustHaveRequiredMembers()
        {
            const string json = @"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'next': 'http://example.com/articles?page[offset]=2',
                    'last': 'http://example.com/articles?page[offset]=10'
                  }
                }";

            var exception = Record.Exception(() => json.Deserialize<JsonApiDocument>());

            Assert.NotNull(exception);
            Assert.Contains("document must contain 'data', 'errors' or 'meta'", exception.Message);
        }

        [Fact(Skip = "Not implemented")]
        public void CanDeserializeSimpleLinks()
        {
            const string json = @"
                {
                  'links': {
                    'self': 'http://example.com/articles',
                    'next': 'http://example.com/articles?page[offset]=2',
                    'prev': 'http://example.com/articles?page[offset]=1',
                    'last': 'http://example.com/articles?page[offset]=10',
                    'first': 'http://example.com/articles?page[offset]=0',
                    'related': 'http://example.com/related'
                  },
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Links);
            Assert.Equal("http://example.com/articles", document.Links.Self.Href);
            Assert.Equal("http://example.com/articles?page[offset]=2", document.Links.Next.Href);
            Assert.Equal("http://example.com/articles?page[offset]=10", document.Links.Last.Href);
            Assert.Equal("http://example.com/articles?page[offset]=1", document.Links.Prev.Href);
            Assert.Equal("http://example.com/articles?page[offset]=0", document.Links.First.Href);
            Assert.Equal("http://example.com/related", document.Links.Related.Href);
        }

        [Fact(Skip = "Not implemented")]
        public void CanDeserializeSimpleNonStandardLink()
        {
            const string json = @"
                {
                  'links': {
                    'articles': 'http://example.com/articles',
                    'blogs': 'http://example.com/blogs'
                  },
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Links);
            Assert.Equal("http://example.com/articles", document.Links["articles"].Href);
            Assert.Equal("http://example.com/blogs", document.Links["blogs"].Href);
        }

        [Fact(Skip = "Not implemented")]
        public void CanDeserializeComplexLinks()
        {
            const string json = @"
                {
                  'links': {
                    'self': {
                      'href': 'http://example.com/articles',
                      'meta': {
                        'count': 10,
                        'title': 'articles'
                      }
                    },
                    'next': {
                      'href': 'http://example.com/articles?page[offset]=2',
                      'meta': {
                        'count': 4,
                        'title': 'blogs'
                      }
                    }
                  },
                  'data': null
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document.Links);

            Assert.Equal("http://example.com/articles", document.Links.Self.Href);
            Assert.Equal(10, document.Links.Self.Meta["count"].GetInt32());
            Assert.Equal("articles", document.Links.Self.Meta["title"].GetString());

            Assert.Equal("http://example.com/articles?page[offset]=2", document.Links.Next.Href);
            Assert.Equal(4, document.Links.Next.Meta["count"].GetInt32());
            Assert.Equal("blogs", document.Links.Next.Meta["title"].GetString());
        }
    }
}
