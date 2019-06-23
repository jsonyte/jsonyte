using System;

namespace Jsonapi
{
    public class JsonApiException : Exception
    {
        public JsonApiException(string message)
            : base(message)
        {
        }
    }
}
