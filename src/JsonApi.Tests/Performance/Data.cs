namespace JsonApi.Tests.Performance
{
    public class Data<T>
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public T Attributes { get; set; }
    }
}
