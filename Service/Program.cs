using System.Reflection;
using AddressBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<AddressBookDbContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")!))
    .AddScoped<IContactsService, ContactsService>()
    .AddRestApi();

var app = builder.Build();
app.UseRestApi();

// Skip database setup when the build launches the app only to extract the OpenAPI document
if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<AddressBookDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
