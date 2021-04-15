namespace Jsonyte.Tests.Models
{
    public interface IAnonymousModelTransformer<T>
    {
        object GetModel(T value);
    }
}
