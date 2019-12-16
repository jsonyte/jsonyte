using System;

namespace JsonApi.AspNetCore
{
    public class JsonApiException : Exception
    {
        public JsonApiException(int statusCode)
        {
        }
    }
}