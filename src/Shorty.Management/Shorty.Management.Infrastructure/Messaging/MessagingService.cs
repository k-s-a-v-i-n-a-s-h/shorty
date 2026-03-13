using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Domain.Options;
using System.Runtime.CompilerServices;

namespace Shorty.Management.Infrastructure.Messaging;

public sealed class MessagingService : IMessagingService
{
    private readonly INatsJSContext _context;

    public MessagingService(INatsConnection connection)
    {
        _context = connection.CreateJetStreamContext();
    }

    public async Task EnsureStreamAsync(string stream, StreamOptions options, CancellationToken cancellationToken = default) =>
        await _context.CreateOrUpdateStreamAsync(new StreamConfig(
            name: stream,
            subjects: options.Subjects)
        {
            MaxBytes = options.MaxBytes,
            Storage = StreamConfigStorage.File,
            Retention = StreamConfigRetention.Limits,
            Discard = StreamConfigDiscard.Old,
            MaxAge = TimeSpan.FromDays(options.MaxAgeDays),
        }, cancellationToken);

    public async Task EnsureConsumerAsync(ConsumerOptions options, CancellationToken cancellationToken = default) =>
        await _context.CreateOrUpdateConsumerAsync(options.Stream, new ConsumerConfig(
            name: options.Name)
        {
            FilterSubjects = options.Subjects,
            MaxDeliver = options.MaxDeliver,
            AckPolicy = ConsumerConfigAckPolicy.Explicit,
            MaxAckPending = options.MaxAckPending,
            AckWait = TimeSpan.FromSeconds(options.AckWaitSeconds),
        }, cancellationToken);

    public async IAsyncEnumerable<Message<T>> ListenAsync<T>(string stream, string consumer, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var natsConsumer = await _context.GetConsumerAsync(stream, consumer, cancellationToken);

        await foreach (var message in natsConsumer.ConsumeAsync<T>(cancellationToken: cancellationToken))
        {
            if (message.Data is null) continue;

            var msgId = message.Headers?["Nats-Msg-Id"].ToString() ?? "unknown";
            var retryCount = message.Metadata?.NumDelivered ?? 1;
            yield return new(msgId, message.Data, retryCount, token => message.AckAsync(cancellationToken: token), (delaySeconds, token) => message.NakAsync(delay: TimeSpan.FromSeconds(delaySeconds), cancellationToken: token));
        }
    }

    public async Task SendAsync<T>(string stream, string subject, T data, string messageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var ack = await _context.PublishAsync(subject, data, cancellationToken: cancellationToken, opts: new NatsJSPubOpts
            {
                MsgId = messageId,
                ExpectedStream = stream,
            });

            ack.EnsureSuccess();
        }
        catch (NatsJSDuplicateMessageException) { }
    }
}
