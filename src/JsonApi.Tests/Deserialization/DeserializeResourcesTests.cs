using System;
using System.Collections.Generic;
using System.Linq;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Deserialization
{
    public class DeserializeResourcesTests
    {
        
        [Fact(Skip = "Not implemented")]
        public void CanDeserializeResourceObjectArray()
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  },
                  {
                    'type': 'articles',
                    'id': '2',
                    'attributes': {
                      'title': 'Jsonapi 2'
                    }
                  }]
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.NotEmpty(articles);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal("Jsonapi", articles[0].Title);

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal("Jsonapi 2", articles[1].Title);
        }

        [Fact(Skip = "Not implemented")]
        public void CanDeserializeResourcesWithMetaArray()
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'meta': {
                      'count': 10
                    }
                  },
                  {
                    'type': 'articles',
                    'id': '2',
                    'meta': {
                      'count': 5
                    }
                  }]
                }";

            var articles = json.Deserialize<ArticleWithMeta[]>();

            Assert.NotNull(articles);
            Assert.Equal(2, articles.Length);

            Assert.Equal("1", articles[0].Id);
            Assert.Equal("articles", articles[0].Type);
            Assert.Equal(10, articles[0].Meta["count"].GetInt32());

            Assert.Equal("2", articles[1].Id);
            Assert.Equal("articles", articles[1].Type);
            Assert.Equal(5, articles[1].Meta["count"].GetInt32());
        }

        [Fact(Skip = "Not implemented")]
        public void CanDeserializeEmptyResourceArray()
        {
            const string json = @"
                {
                  'data': []
                }";

            var articles = json.Deserialize<Article[]>();

            Assert.NotNull(articles);
            Assert.Empty(articles);
        }

        [Theory(Skip = "Not implemented")]
        [InlineData(typeof(List<Article>))]
        [InlineData(typeof(Article[]))]
        [InlineData(typeof(IList<Article>))]
        [InlineData(typeof(ICollection<Article>))]
        [InlineData(typeof(IEnumerable<Article>))]
        public void CanDeserializeCollections(Type type)
        {
            const string json = @"
                {
                  'data': [{
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  },
                  {
                    'type': 'articles',
                    'id': '2',
                    'attributes': {
                      'title': 'Jsonapi 2'
                    }
                  }]
                }";

            var articles = json.Deserialize(type) as IEnumerable<Article>;

            Assert.NotNull(articles);
            Assert.Equal(2, articles.Count());
        }
    }
}
