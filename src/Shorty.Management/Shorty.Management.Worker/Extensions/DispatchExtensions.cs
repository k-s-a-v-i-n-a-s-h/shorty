using Microsoft.EntityFrameworkCore;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Domain.Options;
using Shorty.Management.Infrastructure.Messaging;
using Shorty.Management.Infrastructure.Persistence;
using Shorty.Management.Worker.Options;
using Shorty.Management.Worker.Services;
using Shorty.Management.Worker.Workers;

namespace Shorty.Management.Worker.Extensions;

internal static class DispatchExtensions
{
    public static IServiceCollection AddDispatchServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddPersistence(configuration);
        services.AddSingleton<EmailService>();
        services.AddHostedService<DispatchWorker>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptions<MessagingOptions>().BindConfiguration("Messaging").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<DispatchOptions>().BindConfiguration("Dispatch").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<SmtpOptions>().BindConfiguration("Smtp").ValidateDataAnnotations().ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(configuration.GetConnectionString("Sqlite")));
        services.AddNats(configureOpts: o => o with { Url = configuration.GetConnectionString("Nats")!, SerializerRegistry = NatsJsonSerializerRegistry.Default });
        services.AddSingleton<IMessagingService, MessagingService>();

        return services;
    }
}
