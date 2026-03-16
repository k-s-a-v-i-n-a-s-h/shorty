namespace Shorty.Management.Api.Features.Shorten;

internal sealed record ShortenResult(
    string Path,
    string ShortUrl,
    DateTime ExpiresAt
);

internal static class ShortenModelExtensions
{
    public static ShortenResponse ToDto(this ShortenResult result)
    {
        return new(
            ShortUrl: result.ShortUrl,
            ExpiresAt: result.ExpiresAt
        );
    }
}