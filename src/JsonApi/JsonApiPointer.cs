using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    [JsonConverter(typeof(JsonApiPointerConverter))]
    public class JsonApiPointer
    {
        private readonly string value;

        public JsonApiPointer(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
