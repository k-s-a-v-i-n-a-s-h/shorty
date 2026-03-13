using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;
using Shorty.Management.Infrastructure.Persistence;
using Shorty.Management.Infrastructure.Security;

namespace Shorty.Management.Infrastructure;

public static class Configure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddPersistence(configuration);
        services.AddSecurity();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>().BindConfiguration("Security:Jwt").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<RateLimitOptions>().BindConfiguration("Security:RateLimit").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<HasherOptions>().BindConfiguration("Security:Hasher").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<ExpiryOptions>().BindConfiguration("Expiry").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<AppOptions>().BindConfiguration("App").ValidateDataAnnotations().ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(configuration.GetConnectionString("Sqlite")));
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddSingleton<IHasher, Hasher>();

        return services;
    }
}
