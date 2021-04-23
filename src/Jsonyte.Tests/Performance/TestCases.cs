using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using Jsonyte.Tests.Models;

namespace Jsonyte.Tests.Performance
{
    public static class TestCases
    {
        private static readonly Dictionary<string, Data> Data = new()
        {
            {"Simple", GetSimple()},
            {"Compound", GetCompound()},
            {"LargeCompound", GetLargeCompound()},
            {"ErrorCollection", GetErrorCollection()},
            {"Anonymous", GetAnonymous()}
        };

        public static Data Get(string name)
        {
            return Data[name];
        }

        private static Data GetSimple()
        {
            var model = AutoFaker.Generate<ManyTypesModel>();

            return new Data(model);
        }

        private static Data GetCompound()
        {
            var author = AutoFaker.Generate<Author>();
            var comment = new AutoFaker<Comment>()
                .RuleFor(x => x.Author, author);

            var article = new AutoFaker<ArticleWithAuthor>()
                .RuleFor(x => x.Author, author)
                .RuleFor(x => x.Comments, _ => comment.Generate(2).ToArray())
                .Generate();

            return new Data(article);
        }

        private static Data GetLargeCompound()
        {
            var comment = new AutoFaker<Comment>();
            var articlesArray = new AutoFaker<ArticleWithAuthor>()
                .RuleFor(x => x.Comments, _ => comment.Generate(5).ToArray())
                .Generate(20)
                .ToArray();

            return new Data(articlesArray);
        }

        private static Data GetErrorCollection()
        {
            var errors = new[]
            {
                new JsonApiError
                {
                    Id = "1",
                    Links = new JsonApiErrorLinks
                    {
                        {"next", new JsonApiLink {Href = "http://next"}},
                        {"about", new JsonApiLink {Href = "http://about"}},
                    },
                    Status = "422",
                    Code = "Invalid",
                    Title = "Invalid Attribute",
                    Detail = "First name must contain at least three characters.",
                    Source = new JsonApiErrorSource
                    {
                        Pointer = "/data/attributes/firstName"
                    },
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()},
                        {"name", "first".ToElement()}
                    }
                },
                new JsonApiError
                {
                    Id = "2",
                    Links = new JsonApiErrorLinks
                    {
                        {"next", new JsonApiLink {Href = "http://next2"}},
                        {"about", new JsonApiLink {Href = "http://about2"}},
                    },
                    Status = "522",
                    Code = "Inconceivable",
                    Title = "Inconceivable Attribute",
                    Detail = "Princess bride must be watched.",
                    Source = new JsonApiErrorSource
                    {
                        Pointer = "/data/attributes/princess"
                    },
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()}
                    }
                }
            };

            return new Data(errors);
        }

        public static Data GetAnonymous()
        {
            object GetAuthor(string id, string name)
            {
                return new
                {
                    id,
                    type = "people",
                    name
                };
            }

            object GetAuthors()
            {
                return new[] {("2", "Bill"), ("3", "Ted")}.Select(x => GetAuthor(x.Item1, x.Item2));
            }

            object GetTags()
            {
                return new[] {"tag1", "tag2"}.Select(x => x);
            }

            object GetArticle()
            {
                return new
                {
                    id = "1",
                    type = "articles",
                    title = "Jsonapi",
                    authors = GetAuthors(),
                    tags = GetTags()
                };
            }

            return new Data(GetArticle()) {SkipDeserialize = true};
        }
    }
}
