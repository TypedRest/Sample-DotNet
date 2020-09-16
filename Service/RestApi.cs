using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;

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
                .Configure<MvcOptions>(opts => opts.Filters.Add(typeof(ApiExceptionFilterAttribute)))
                .AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

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
