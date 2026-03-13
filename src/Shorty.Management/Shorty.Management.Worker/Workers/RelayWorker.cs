using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Domain.Options;
using Shorty.Management.Infrastructure.Persistence;
using Shorty.Management.Worker.Models;
using Shorty.Management.Worker.Options;

namespace Shorty.Management.Worker.Workers;

internal sealed class RelayWorker : BackgroundService
{
    private readonly TimeSpan _pollingInterval;
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<RelayWorker> _logger;
    private readonly RelayOptions _relayOptions;
    private readonly StreamOptions _streamOptions;
    private readonly Subject _subject;

    public RelayWorker(IDbContextFactory<ApplicationDbContext> factory, IOptions<MessagingOptions> messagingOptions, IMessagingService messagingService, IOptions<RelayOptions> relayOptions, ILogger<RelayWorker> logger)
    {
        var subject = "Email";
        _subject = messagingOptions.Value.Subjects.GetValueOrDefault(subject)
            ?? throw new KeyNotFoundException($"Subject '{subject}' is not defined.");

        _streamOptions = messagingOptions.Value.Streams.GetValueOrDefault(_subject.Stream)
            ?? throw new KeyNotFoundException($"Stream '{_subject.Stream}' is not defined");

        _pollingInterval = TimeSpan.FromSeconds(relayOptions.Value.PollingIntervalSeconds);
        _relayOptions = relayOptions.Value;
        _factory = factory;
        _messagingService = messagingService;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messagingService.EnsureStreamAsync(_subject.Stream, _streamOptions, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
                await Task.Delay(_pollingInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retrying in {Interval}s...", _pollingInterval.TotalSeconds * 2);
                await Task.Delay(_pollingInterval * 2, stoppingToken);
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken stoppingToken)
    {
        using var db = _factory.CreateDbContext();

        var messages = await db.Outbox
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.CreatedAt)
            .Take(_relayOptions.BatchSize)
            .ToListAsync(stoppingToken);

        if (messages.Count != 0)
        {
            await Parallel.ForEachAsync(messages, new ParallelOptions
            {
                MaxDegreeOfParallelism = _relayOptions.MaxConcurrency,
                CancellationToken = stoppingToken
            }, async (message, token) =>
            {
                try
                {
                    await SendMessageAsync(message, token);
                    message.Processed();
                }
                catch (Exception ex)
                {
                    message.Error(ex.Message);
                }
            });

            await db.SaveChangesAsync(stoppingToken);
        }
    }

    private async Task SendMessageAsync(Outbox message, CancellationToken token)
    {
        var email = new EmailMessage
        {
            From = new(_relayOptions.SenderName, _relayOptions.SenderEmail),
            To = message.Recipient,
            Subject = message.Subject,
            HtmlBody = message.HtmlBody,
        };

        await _messagingService.SendAsync(_subject.Stream, _subject.Value, email, message.Id.ToString(), token);
    }
}
