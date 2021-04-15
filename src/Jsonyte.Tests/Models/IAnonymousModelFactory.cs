namespace Jsonyte.Tests.Models
{
    public interface IAnonymousModelFactory
    {
        IAnonymousModelTransformer<T> GetTransformer<T>();
    }
}
