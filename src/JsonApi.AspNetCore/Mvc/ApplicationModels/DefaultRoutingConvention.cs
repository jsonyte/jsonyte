using System;
using System.Linq;
using System.Reflection;
using JsonApi.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace JsonApi.AspNetCore.Mvc.ApplicationModels
{
    public class DefaultRoutingConvention : IApplicationModelConvention
    {
        private readonly IRouteNameFormatter routeNameFormatter;

        public DefaultRoutingConvention(IRouteNameFormatter routeNameFormatter)
        {
            this.routeNameFormatter = routeNameFormatter;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var resourceType = GetControllerResourceType(controller.ControllerType);

                if (resourceType == null || RouteOverridden(controller.ControllerType))
                {
                    continue;
                }

                var routeName = routeNameFormatter.FormatResource(resourceType);

                controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel {Template = routeName};
            }
        }

        private bool RouteOverridden(Type type)
        {
            return type.GetCustomAttributes<RouteAttribute>(true).Any();
        }

        private Type GetControllerResourceType(Type type)
        {
            var currentType = type;

            while (currentType != null && (!currentType.IsGenericType || currentType.GetGenericTypeDefinition() != typeof(JsonApiController<>)))
            {
                currentType = currentType.BaseType;
            }

            if (currentType == null)
            {
                return null;
            }

            return currentType.GetGenericArguments().FirstOrDefault();
        }
    }
}