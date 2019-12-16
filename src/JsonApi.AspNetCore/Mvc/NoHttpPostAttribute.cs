using JsonApi.AspNetCore.Mvc.Filters;

namespace JsonApi.AspNetCore.Mvc
{
    public class NoHttpPostAttribute : HttpRestrictionAttribute
    {
        protected override bool CanExecute(string method)
        {
            return method != "POST";
        }
    }
}