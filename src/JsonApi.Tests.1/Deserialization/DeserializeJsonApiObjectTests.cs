using System;
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
        [InlineData("1.0")]
        [InlineData("1.1")]
        [InlineData("1.1.1")]
        [InlineData("2.0.1")]
        public void CanConvertNewJsonApiVersions(string version)
        {
            var document = Json.Format(version).Deserialize<Document>();

            Assert.NotNull(document.JsonApi);
            Assert.Equal(document.JsonApi.Version, Version.Parse(version));
        }

        [Theory]
        [InlineData("1.b")]
        [InlineData("1.0-beta.1")]
        [InlineData("abcdef")]
        [InlineData("1.#.0")]
        public void InvalidVersionThrows(string version)
        {
            var exception = Record.Exception(() => Json.Format(version).Deserialize<Document>());

            Assert.IsType<JsonApiException>(exception);
            Assert.Contains("invalid", exception.Message.ToLower());
        }

        [Theory]
        [InlineData("0.9")]
        [InlineData("0.1")]
        [InlineData("0.0.1")]
        [InlineData("0.9.9")]
        public void LessThanMinimumVersionThrows(string version)
        {
            var exception = Record.Exception(() => Json.Format(version).Deserialize<Document>());

            Assert.IsType<JsonApiException>(exception);
            Assert.Contains("minimum required", exception.Message.ToLower());
        }

        private class Document
        {
            [JsonPropertyName("jsonapi")]
            public JsonApiObject JsonApi { get; set; }
        }
    }
}
