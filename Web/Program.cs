using AddressBook;
using AddressBook.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("main");

// The page is served from the same origin as the API
builder.Services.AddSingleton<IAddressBookClient>(new AddressBookClient(new Uri(builder.HostEnvironment.BaseAddress)));

await builder.Build().RunAsync();
