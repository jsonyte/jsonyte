namespace Jsonyte.Tests.Models
{
    public class AnonymousArticleWithAuthorTransformer : IAnonymousModelTransformer<ArticleWithAuthor>
    {
        public object GetModel(ArticleWithAuthor value)
        {
            return new
            {
                id = value.Id,
                type = value.Type,
                title = value.Title,
                author = new
                {
                    id = value.Author.Id,
                    type = value.Author.Type,
                    name = value.Author.Name
                }
            };
        }
    }
}
