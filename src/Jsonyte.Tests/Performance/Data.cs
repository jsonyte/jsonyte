using System;
using System.Text;
using System.Text.Json;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonApi.Tests.Performance
{
    public class Data
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions().AddJsonApi();

        private readonly JsonApiSerializerSettings settings = new();

        public Data(object value)
        {
            Value = value;
            Type = value.GetType();
            Json = JsonSerializer.Serialize(value, value.GetType());
            JsonApi = JsonConvert.SerializeObject(value, settings);
            JsonBytes = Encoding.UTF8.GetBytes(Json);
            JsonApiBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, value.GetType(), options));
        }

        public object Value { get; }

        public Type Type { get; }

        public string Json { get; }

        public string JsonApi { get; }

        public byte[] JsonBytes { get; }

        public byte[] JsonApiBytes { get; }
    }
}
