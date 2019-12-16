using System;

namespace JsonApi.AspNetCore.Mvc.Formatters
{
    public interface IRouteNameFormatter
    {
        string FormatResource(Type type);
    }
}