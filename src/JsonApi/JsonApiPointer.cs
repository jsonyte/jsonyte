namespace JsonApi
{
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
