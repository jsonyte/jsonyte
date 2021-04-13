using System;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Serialization
{
    public class SerializeLinksTests
    {
        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeSimpleLinks(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Links = new JsonApiDocumentLinks
                {
                    Self = "http://example.com/articles",
                    Next = "http://example.com/articles?page[offset]=2",
                    Prev = "http://example.com/articles?page[offset]=1",
                    Last = "http://example.com/articles?page[offset]=10",
                    First = "http://example.com/articles?page[offset]=0",
                    Related = "http://example.com/related"
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeSimpleNonStandardLink(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Links = new JsonApiDocumentLinks
                {
                    {"articles", "http://example.com/articles"},
                    {"blogs", "http://example.com/blogs"}
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
                {
                  'links': {
                    'articles': 'http://example.com/articles',
                    'blogs': 'http://example.com/blogs'
                  },
                  'data': null
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanSerializeComplexLinks(Type documentType)
        {
            var document = new MockJsonApiDocument
            {
                Links = new JsonApiDocumentLinks
                {
                    Self = new JsonApiLink
                    {
                        Href = "http://example.com/articles",
                        Meta = new JsonApiMeta
                        {
                            {"count", 10.ToElement()},
                            {"title", "articles".ToElement()}
                        }
                    },
                    Next = new JsonApiLink
                    {
                        Href = "http://example.com/articles?page[offset]=2",
                        Meta = new JsonApiMeta
                        {
                            {"count", 4.ToElement()},
                            {"title", "blogs".ToElement()}
                        }
                    }
                }
            };

            var json = document.SerializeDocument(documentType);

            Assert.Equal(@"
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
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
