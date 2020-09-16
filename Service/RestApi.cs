using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace AddressBook
{
    public static class RestApi
    {
        /// <summary>
        /// Adds services for serving REST APIs via MVC controllers.
        /// </summary>
        public static IMvcBuilder AddRestApi(this IServiceCollection services)
            => services
                .AddSwaggerGen(opts =>
                {
                    opts.IncludeXmlComments(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "AddressBook.xml"));
                    opts.IncludeXmlComments(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "AddressBook.Dto.xml"));
                })
                .AddSwaggerGenNewtonsoftSupport()
                .Configure<MvcOptions>(opts => opts.Filters.Add(typeof(ApiExceptionFilterAttribute)))
                .AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

        /// <summary>
        /// Registers endpoints for REST API controllers.
        /// </summary>
        public static IApplicationBuilder UseRestApi(this IApplicationBuilder app)
            => app
                .UseSwagger()
                .UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Address Book"))
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
