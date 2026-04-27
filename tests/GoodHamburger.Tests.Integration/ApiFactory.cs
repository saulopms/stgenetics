using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace GoodHamburger.Tests.Integration;

/// <summary>Creates an in-process test server sharing the real Program pipeline.</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

    /// <summary>Creates a client that bypasses all default request headers (no auth).</summary>
    public HttpClient CreateAnonymousClient() => Server.CreateClient();
}
