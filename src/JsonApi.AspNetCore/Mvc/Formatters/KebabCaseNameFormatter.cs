using System;
using JsonApi.AspNetCore.Extensions;

namespace JsonApi.AspNetCore.Mvc.Formatters
{
    public class KebabCaseNameFormatter : IRouteNameFormatter
    {
        public string FormatResource(Type type)
        {
            return type.Name.Dasherize();
        }
    }
}