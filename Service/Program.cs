using System.Reflection;
using AddressBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<AddressBookDbContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")!))
    .AddScoped<IContactsService, ContactsService>()
    .AddRestApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();
app.UseDefaultFiles(); // Rewrites / to /index.html, so must come before routing
app.UseRouting();
app.MapRestApi();
app.MapStaticAssets(); // Serves the Blazor WebAssembly page from Web/

// Skip database setup when the build launches the app only to extract the OpenAPI document
if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<AddressBookDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
