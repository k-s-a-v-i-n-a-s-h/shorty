using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Domain.Options;
using Shorty.Management.Worker.Models;
using Shorty.Management.Worker.Options;
using Shorty.Management.Worker.Services;

namespace Shorty.Management.Worker.Workers;

internal sealed class DispatchWorker : BackgroundService
{
    private readonly EmailService _emailService;
    private readonly IMessagingService _messagingService;
    private readonly ILogger<DispatchWorker> _logger;
    private readonly DispatchOptions _dispatchOptions;
    private readonly ConsumerOptions _consumerOptions;

    public DispatchWorker(EmailService emailService, IOptions<MessagingOptions> messagingOptions, IMessagingService messagingService, IOptions<DispatchOptions> dispatchOptions, ILogger<DispatchWorker> logger)
    {
        var consumer = "Email";
        _consumerOptions = messagingOptions.Value.Consumers.GetValueOrDefault(consumer)
            ?? throw new KeyNotFoundException($"Consumer '{consumer}' is not defined.");

        _dispatchOptions = dispatchOptions.Value;
        _emailService = emailService;
        _messagingService = messagingService;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messagingService.EnsureConsumerAsync(_consumerOptions, cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Parallel.ForEachAsync(_messagingService.ListenAsync<EmailMessage>(
                    _consumerOptions.Stream,
                    _consumerOptions.Name,
                    stoppingToken), new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _dispatchOptions.MaxConcurrency
                    }, async (message, token) =>
                    {
                        await ProcessMessageAsync(message, token);
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retrying in {Interval}s...", _dispatchOptions.ErrorBackoffSeconds);
                await Task.Delay(_dispatchOptions.ErrorBackoffSeconds, stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message<EmailMessage> message, CancellationToken token)
    {
        (object client, Func<Task> cleanup) connection = default;

        try
        {
            connection = await _emailService.ConnectAsync(token);
            await _emailService.SendAsync(connection.client, message.Data, token);
            await message.Ack(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read message with ID {Id}", message.Id);
            await message.Nak(_dispatchOptions.ErrorBackoffSeconds, token);
        }
        finally
        {
            if (connection.cleanup is not null)
                await connection.cleanup();
        }
    }
}
