using JsonApi.AspNetCore.Mvc.ApplicationModels;
using JsonApi.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace JsonApi.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonApi(this IServiceCollection services)
        {
            services.AddMvcCore()
                .AddMvcOptions(x =>
                {
                    x.Conventions.Insert(0, new DefaultRoutingConvention(new KebabCaseNameFormatter()));
                });

            return services;
        }
    }
}