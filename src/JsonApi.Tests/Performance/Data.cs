using System.Text;
using System.Text.Json;

namespace JsonApi.Tests.Performance
{
    public class Data<T>
    {
        public Data(T value, JsonSerializerOptions options)
        {
            Value = value;

            Json = JsonSerializer.Serialize(value);
            JsonApi = JsonSerializer.Serialize(value, options);
            JsonBytes = Encoding.UTF8.GetBytes(Json);
            JsonApiBytes = Encoding.UTF8.GetBytes(JsonApi);
        }

        public T Value { get; }

        public string Json { get; }

        public string JsonApi { get; }

        public byte[] JsonBytes { get; }

        public byte[] JsonApiBytes { get; }
    }
}
