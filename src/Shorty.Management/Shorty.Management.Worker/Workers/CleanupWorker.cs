using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shorty.Management.Infrastructure.Persistence;
using Shorty.Management.Worker.Options;

namespace Shorty.Management.Worker.Workers;

internal sealed class CleanupWorker : BackgroundService
{
    private readonly TimeSpan _pollingInterval;
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly ILogger<CleanupWorker> _logger;
    private readonly CleanupOptions _options;

    public CleanupWorker(IOptions<CleanupOptions> options, IDbContextFactory<ApplicationDbContext> factory, ILogger<CleanupWorker> logger)
    {
        _options = options.Value;
        _pollingInterval = TimeSpan.FromHours(_options.PollingIntervalHours);
        _factory = factory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var db = _factory.CreateDbContext();

                await DeleteUnverifiedUsersAsync(db, stoppingToken);
                await DeleteProcessedOutboxAsync(db, stoppingToken);
                await DeleteExpiredLinksAsync(db, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }

    private async Task<int> DeleteUnverifiedUsersAsync(ApplicationDbContext db, CancellationToken token)
    {
        var cutoff = DateTime.UtcNow.Subtract(TimeSpan.FromDays(_options.UserRetentionDays));

        return await db.Users
            .Where(u => !u.IsVerified && u.CreatedAt < cutoff)
            .ExecuteDeleteAsync(token);
    }

    private async Task<int> DeleteProcessedOutboxAsync(ApplicationDbContext db, CancellationToken token)
    {
        var cutoff = DateTime.UtcNow.Subtract(TimeSpan.FromDays(_options.OutboxRetentionDays));

        return await db.Outbox
            .Where(o => o.CreatedAt < cutoff && (o.ProcessedAt != null || o.LastError != null))
            .ExecuteDeleteAsync(token);
    }

    private static Task<int> DeleteExpiredLinksAsync(ApplicationDbContext db, CancellationToken token) =>
        db.Links.Where(l => l.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(token);
}
