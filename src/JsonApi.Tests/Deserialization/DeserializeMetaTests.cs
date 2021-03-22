using System;
using System.Linq;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeMetaTests
    {
        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanDeserializeOnlyMeta(Type documentType)
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

            var document = json.DeserializeDocument(documentType);

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

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanDeserializeErrorsWithMeta(Type documentType)
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

            var document = json.DeserializeDocument(documentType);

            Assert.NotNull(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanDeserializeDataWithMeta(Type documentType)
        {
            const string json = @"
                {
                  'data': null,
                  'meta': {
                    'name': 'Bloggs',
                    'active': true
                  }
                }";

            var document = json.DeserializeDocument(documentType);

            Assert.Null(document.Errors);
            Assert.NotNull(document.Meta);
            Assert.Null(document.Data);

            Assert.Equal("Bloggs", document.Meta["name"].GetString());
            Assert.True(document.Meta["active"].GetBoolean());
        }
    }
}
