using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AddressBook;

/// <summary>
/// Sets up an in-memory version of the ASP.NET MVC stack for decoupled testing of controllers and the client library.
/// </summary>
public abstract class ApiFactsBase : IDisposable
{
    private readonly IHost _host;
    private readonly TestServer _server;

    protected ApiFactsBase(ITestOutputHelper output)
    {
        _host = CreateHostBuilder(output).Start();
        _server = _host.GetTestServer();
        Client = new AddressBookClient(_server.CreateClient(), new Uri("http://localhost"));
    }

    private IHostBuilder CreateHostBuilder(ITestOutputHelper output)
        => new HostBuilder().ConfigureWebHost(x => x
            .UseTestServer()
            .ConfigureLogging(builder => builder.AddXUnit(output))
            .ConfigureServices((context, services) => services
                .AddRestApi()
                .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(RestApi).Assembly)))
            .ConfigureServices(ConfigureService)
            .Configure(builder => builder
                .UseRestApi()));

    /// <summary>
    /// Registers dependencies for controllers.
    /// </summary>
    protected abstract void ConfigureService(IServiceCollection services);

    /// <summary>
    /// A message handler configured for in-memory communication with ASP.NET MVC controllers.
    /// </summary>
    protected readonly IAddressBookClient Client;

    public virtual void Dispose()
    {
        _server.Dispose();
        _host.Dispose();
    }
}
