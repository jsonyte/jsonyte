using Newtonsoft.Json;
using Xunit;

namespace Jsonapi.Tests
{
    public class JsonApiSerializerSettingsTests
    {
        [Fact]
        public void CanConvertData()
        {
            var article = JsonConvert.DeserializeObject<Article>(@"
                {
                  'data': {
                    'type': 'articles',
                    'id': '1',
                    'attributes': {
                      'title': 'My article'
                    },
                    relationships: {
                      'author': {
                        'data': {
                          'type': 'authors',
                          'id': '2'
                        }
                      }
                    }
                  },
                  included: [
                    {
                      'type': 'authors',
                      'id': '2',
                      'attributes': {
                        'name': 'Rob'
                      }
                    }
                  ]
                }", new JsonApiSerializerSettings());
        }

        [Fact]
        public void CanWriteData()
        {
            var article = new Article
            {
                Id = 1,
                Title = "My article",
                Author = new Author
                {
                    Id = 2,
                    Name = "Rob",
                    Country = new Country
                    {
                        Id = 3,
                        Zone = "NZ"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(article, new JsonApiSerializerSettings());
        }

        private class Article
        {
            public int Id { get; set; }

            public string Type { get; } = "articles";

            public string Title { get; set; }

            public Author Author { get; set; }
        }

        private class Author
        {
            public int Id { get; set; }

            public string Type { get; } = "authors";

            public string Name { get; set; }

            public Country Country { get; set; }
        }

        private class Country
        {
            public int Id { get; set; }

            public string Type { get; } = "country";

            public string Zone { get; set; }
        }
    }
}
