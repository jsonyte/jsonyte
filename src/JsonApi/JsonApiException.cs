using System;

namespace JsonApi
{
    public class JsonApiException : Exception
    {
        public JsonApiException(string message)
            : base(message)
        {
        }
    }
}
