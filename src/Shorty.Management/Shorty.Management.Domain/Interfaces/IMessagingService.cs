using Shorty.Management.Domain.Options;

namespace Shorty.Management.Domain.Interfaces;

public record Message<T>(string Id, T Data, ulong DeliveryCount, Func<CancellationToken, ValueTask> Ack, Func<int, CancellationToken, ValueTask> Nak);

public interface IMessagingService
{
    Task EnsureStreamAsync(string stream, StreamOptions options, CancellationToken cancellationToken = default);
    Task EnsureConsumerAsync(ConsumerOptions options, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Message<T>> ListenAsync<T>(string stream, string consumer, CancellationToken cancellationToken = default);
    Task SendAsync<T>(string stream, string subject, T data, string messageId, CancellationToken cancellationToken = default);
}
