using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using TestcontainersATW.Persistence;
using TestcontainersATW.Settings;

namespace TestcontainersATW.IT;

// ReSharper disable once ClassNeverInstantiated.Global
public class AzurTechWinterApiFactory :
    WebApplicationFactory<IAzurTechWinterAnchor>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithDatabase("postgres")
        .WithPortBinding(4321, 5432)
        .Build();

    // private readonly IContainer _customContainer = new ContainerBuilder()
    //     .WithImage("azurtechwinter")
    //     .WithImagePullPolicy(PullPolicy.Never)
    //     .WithPortBinding(3000, 80)
    //     //.WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
    //     .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, conf) => { conf.AddJsonFile("appsettings.Test.json").Build(); });
        builder.ConfigureLogging(logging => { logging.ClearProviders(); });
        builder.ConfigureTestServices(services =>
        {
            services.Configure<ConnectionStrings>(strings =>
                strings.AzurTechWinter = _postgreSqlContainer.GetConnectionString());

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<AzurTechWinterContext>();
            context.Database.EnsureCreated();
        });
    }


    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        // await _customContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        // await _customContainer.StopAsync();
    }
}