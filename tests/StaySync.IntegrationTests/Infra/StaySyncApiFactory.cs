using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StaySync.Infrastructure.Persistence;

namespace StaySync.IntegrationTests.Infra;

public sealed class StaySyncApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    private readonly string _conn = connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 1) Set environment & force the setting with highest precedence
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Sql", _conn); // host settings > env vars > json

        // 2) (optional) still add an in-memory config override
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Sql"] = _conn
            });
        });

        // 3) Replace the DbContextOptions so EF absolutely uses our connection string
        builder.ConfigureServices(services =>
        {
            // Remove any previous DbContextOptions<StaySyncDbContext>
            var dbCtxOptions = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<StaySyncDbContext>));
            if (dbCtxOptions is not null) services.Remove(dbCtxOptions);

            // Add DbContext with our Testcontainers connection
            services.AddDbContext<StaySyncDbContext>(opt =>
            {
                opt.UseSqlServer(_conn, sql => sql.EnableRetryOnFailure(5));
                opt.EnableSensitiveDataLogging(false);
            });
        });
    }

    public HttpClient CreateAuthedClient(string apiKey = "demo-key")
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
}
