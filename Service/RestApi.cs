using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace AddressBook;

public static class RestApi
{
    /// <summary>
    /// Adds services for serving REST APIs via MVC controllers.
    /// </summary>
    public static IMvcBuilder AddRestApi(this IServiceCollection services)
        => services
            .AddOpenApi(opts =>
            {
                opts.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info.Title = "Address Book";
                    document.Info.Description = "Simple REST API for storing contacts in an address book.";
                    return Task.CompletedTask;
                });
            })
            .Configure<MvcOptions>(opts => opts.Filters.Add(typeof(ApiExceptionFilterAttribute)))
            .AddControllers();

    /// <summary>
    /// Registers endpoints for REST API controllers, the OpenAPI document and the API reference UI.
    /// </summary>
    public static IApplicationBuilder UseRestApi(this IApplicationBuilder app)
        => app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapOpenApi();
                endpoints.MapScalarApiReference(opts => opts.WithTitle("Address Book"));
            });
}
