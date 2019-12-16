using System;
using System.Linq;
using System.Reflection;
using JsonApi.AspNetCore.Mvc;
using JsonApi.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JsonApiWeb.Filters
{
    public class RemoveVerbsFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var description in context.ApiDescriptions)
            {
                if (description.ActionDescriptor is ControllerActionDescriptor descriptor)
                {
                    var key = "/" + description.RelativePath.TrimEnd('/');
                    var path = swaggerDoc.Paths[key];

                    if (path != null)
                    {
                        if (PostHidden(descriptor))
                        {
                            path.Operations.Remove(OperationType.Post);
                        }
                    }
                }
            }
        }

        private bool PostHidden(ControllerActionDescriptor descriptor)
        {
            return descriptor.ControllerTypeInfo.GetCustomAttributes<NoHttpPostAttribute>().Any() ||
                   descriptor.MethodInfo.GetCustomAttributes<NoHttpPostAttribute>().Any();
        }
    }
}