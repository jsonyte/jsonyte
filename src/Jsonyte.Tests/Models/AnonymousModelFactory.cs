using System;
using System.Collections.Generic;

namespace Jsonyte.Tests.Models
{
    public class AnonymousModelFactory : IAnonymousModelFactory
    {
        private readonly Dictionary<Type, object> models = new()
        {
            {typeof(Article), new AnonymousArticleTransformer()},
            {typeof(ArticleWithAuthor), new AnonymousArticleWithAuthorTransformer()}
        };

        public IAnonymousModelTransformer<T> GetTransformer<T>()
        {
            if (models.TryGetValue(typeof(T), out var model))
            {
                return model as IAnonymousModelTransformer<T>;
            }

            throw new InvalidOperationException();
        }
    }
}
