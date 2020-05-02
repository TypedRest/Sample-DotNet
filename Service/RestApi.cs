using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
                .Configure<MvcOptions>(opts => opts.Filters.Add(typeof(ApiExceptionFilterAttribute)))
                .AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

        /// <summary>
        /// Registers endpoints for REST API controllers.
        /// </summary>
        public static IApplicationBuilder UseRestApi(this IApplicationBuilder app)
            => app
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
