namespace Jsonyte.Tests.Models
{
    public class AnonymousArticleTransformer : IAnonymousModelTransformer<Article>
    {
        public object GetModel(Article value)
        {
            return new
            {
                id = value.Id,
                type = value.Type,
                title = value.Title
            };
        }
    }
}
