using Microsoft.EntityFrameworkCore;
using Shorty.Management.Infrastructure.Persistence;
using Shorty.Management.Worker.Options;
using Shorty.Management.Worker.Workers;

namespace Shorty.Management.Worker.Extensions;

internal static class CleanupExtensions
{
    public static IServiceCollection AddCleanupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddPersistence(configuration);
        services.AddHostedService<CleanupWorker>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<CleanupOptions>().BindConfiguration("Cleanup").ValidateDataAnnotations().ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(o => o.UseSqlite(configuration.GetConnectionString("Sqlite")));

        return services;
    }
}
