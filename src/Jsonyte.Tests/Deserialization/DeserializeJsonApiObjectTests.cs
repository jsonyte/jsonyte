using System;
using System.Text.Json.Serialization;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Deserialization
{
    public class DeserializeJsonApiObjectTests
    {
        private const string Json = @"
            {{
              'jsonapi': {{
                'version': '{0}'
              }}
            }}";

        [Theory]
        [InlineData("0.1")]
        [InlineData("1.0")]
        [InlineData("1.1")]
        [InlineData("1.1.1")]
        [InlineData("2.0")]
        [InlineData("2.0.1")]
        public void CanConvertJsonApiVersions(string version)
        {
            var document = string.Format(Json, version).Deserialize<Document>();

            Assert.NotNull(document.JsonApi);
            Assert.Equal(document.JsonApi.Version, version);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanDeserializeObjectInDocument(Type documentType)
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                    'version': '1.0',
                    'meta': {
                      'count': 10,
                      'feature': 'something'
                    }
                  }
                }";

            var document = json.DeserializeDocument(documentType);

            Assert.NotNull(document);
            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
            Assert.Equal(10, document.JsonApi.Meta?["count"].GetInt32());
            Assert.Equal("something", document.JsonApi.Meta?["feature"].GetString());
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void DeserializingJsonApiWithNoMembersHasDefaultVersion(Type documentType)
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                  }
                }";

            var document = json.DeserializeDocument(documentType);

            Assert.NotNull(document);
            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
            Assert.Null(document.JsonApi.Meta);
        }

        [Theory]
        [InlineData(typeof(JsonApiDocument))]
        [InlineData(typeof(JsonApiDocument<Article>))]
        public void CanDeserializeJsonApiWithOnlyMetaAndDefaultVersion(Type documentType)
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                    'meta': {
                      'count': 10,
                      'feature': 'something'
                    }
                  }
                }";

            var document = json.DeserializeDocument(documentType);

            Assert.NotNull(document);
            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
            Assert.Equal(10, document.JsonApi.Meta?["count"].GetInt32());
            Assert.Equal("something", document.JsonApi.Meta?["feature"].GetString());
        }

        private class Document
        {
            [JsonPropertyName("jsonapi")]
            public JsonApiObject JsonApi { get; set; }
        }
    }
}
