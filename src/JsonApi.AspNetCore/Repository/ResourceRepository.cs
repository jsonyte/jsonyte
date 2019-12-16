using System.Collections.Generic;
using System.Linq;

namespace JsonApi.AspNetCore.Repository
{
    public class ResourceRepository<T> : IRepository<T>
    {
        public virtual IEnumerable<T> GetAsync()
        {
            return Enumerable.Empty<T>();
        }

        public virtual T GetAsync(string id)
        {
            return default;
        }

        public virtual void Create(T model)
        {
        }

        public virtual void Update(string id, T model)
        {
        }

        public virtual void Delete(string id)
        {
        }
    }
}