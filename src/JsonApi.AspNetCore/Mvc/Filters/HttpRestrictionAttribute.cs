using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApi.AspNetCore.Mvc.Filters
{
    public abstract class HttpRestrictionAttribute : ActionFilterAttribute
    {
        protected abstract bool CanExecute(string method);

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;

            if (!CanExecute(method))
            {
                throw new JsonApiException(StatusCodes.Status405MethodNotAllowed);
            }

            await next();
        }
    }
}