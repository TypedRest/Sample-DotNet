using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddressBook;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Registers services for dependency injection.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
        => services
            .AddScoped<IContactsService, ContactsService>()
            .AddDbContext<DbContext>(opts => opts.UseSqlite(_configuration.GetConnectionString("Database")))
            .AddRestApi();

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    public void Configure(IApplicationBuilder app)
        => app.UseRestApi();
}
