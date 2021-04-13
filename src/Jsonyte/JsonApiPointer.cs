namespace Jsonyte
{
    public sealed class JsonApiPointer
    {
        private readonly string value;

        public JsonApiPointer(string value)
        {
            this.value = value;
        }

        public static implicit operator JsonApiPointer(string value)
        {
            return new(value);
        }

        public static implicit operator string(JsonApiPointer value)
        {
            return value.ToString();
        }

        public override string ToString()
        {
            return value;
        }
    }
}
