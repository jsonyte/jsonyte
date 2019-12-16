using System.Collections.Generic;

namespace JsonApi.AspNetCore.Repository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAsync();

        T GetAsync(string id);

        void Create(T model);

        void Update(string id, T model);

        void Delete(string id);
    }
}