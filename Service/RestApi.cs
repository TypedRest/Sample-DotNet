using Microsoft.AspNetCore.Mvc;

namespace AddressBook;

public static class RestApi
{
    /// <summary>
    /// Adds services for serving REST APIs via MVC controllers.
    /// </summary>
    public static IMvcBuilder AddRestApi(this IServiceCollection services)
        => services
            .AddSwaggerGen(opts =>
            {
                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AddressBook.xml"));
                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AddressBook.Dto.xml"));
            })
            .Configure<MvcOptions>(opts => opts.Filters.Add(typeof(ApiExceptionFilterAttribute)))
            .AddControllers();

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
