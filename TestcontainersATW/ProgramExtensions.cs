using TestcontainersATW.Persistence;
using TestcontainersATW.Settings;

namespace TestcontainersATW;

public static class ProgramExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConnectionStrings>(connectionStrings =>
        {
            connectionStrings.AzurTechWinter = configuration.GetConnectionString("AzurTechWinter")!;
        });

        services.AddDbContext<AzurTechWinterContext>();
    }
}