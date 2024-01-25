using AddressBook;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<AddressBookDbContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")!))
    .AddScoped<IContactsService, ContactsService>()
    .AddRestApi();

var app = builder.Build();
app.UseRestApi();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetRequiredService<AddressBookDbContext>())
    context.Database.EnsureCreated();

app.Run();
