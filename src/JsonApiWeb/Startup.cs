using JsonApi;
using JsonApi.AspNetCore.Extensions;
using JsonApi.AspNetCore.Repository;
using JsonApiWeb.Filters;
using JsonApiWeb.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace JsonApiWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddJsonApi()
                .AddControllers();
            //.AddJsonOptions(x => x.JsonSerializerOptions.Converters);

            services.AddSingleton<IRepository<Todo>, TodoRepository>();

            services.AddSwaggerGen(x =>
            {
                x.DocumentFilter<RemoveVerbsFilter>();
                x.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
            });
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var h = app.ApplicationServices.GetService(typeof(IJsonHelper));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(x =>
            {
                x.MapControllers();
            });
        }
    }
}