using System.Text.Json.Serialization;
using Xunit;

namespace JsonApi.Tests.Deserialization
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
            var document = Json.Format(version).Deserialize<Document>();

            Assert.NotNull(document.JsonApi);
            Assert.Equal(document.JsonApi.Version, version);
        }

        [Fact]
        public void CanDeserializeObjectInDocument()
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

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document);
            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
            Assert.Equal(10, document.JsonApi.Meta?["count"].GetInt32());
            Assert.Equal("something", document.JsonApi.Meta?["feature"].GetString());
        }

        [Fact]
        public void DeserializingJsonApiWithNoMembersHasDefaultVersion()
        {
            const string json = @"
                {
                  'data': null,
                  'jsonapi': {
                  }
                }";

            var document = json.Deserialize<JsonApiDocument>();

            Assert.NotNull(document);
            Assert.NotNull(document.JsonApi);
            Assert.Equal("1.0", document.JsonApi.Version);
            Assert.Null(document.JsonApi.Meta);
        }

        [Fact]
        public void CanDeserializeJsonApiWithOnlyMetaAndDefaultVersion()
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

            var document = json.Deserialize<JsonApiDocument>();

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
