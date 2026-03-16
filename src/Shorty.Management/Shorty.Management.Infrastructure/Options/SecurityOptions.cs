using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Infrastructure.Options;

public sealed class JwtOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "Expiry must be at least 1 minute.")]
    public int ExpiryMinutes { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required, MinLength(32, ErrorMessage = "Key must be at least 32 characters for security.")]
    public required string Key { get; init; }
}

public sealed class RateLimitOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "Permit limit must be at least 1 request.")]
    public required int PermitLimit { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Window must be at least 1 minute.")]
    public required int WindowMinutes { get; init; }
}

public sealed class HasherOptions
{
    [Required, MinLength(32, ErrorMessage = "Secret must be at least 32 characters for security.")]
    public required string Secret { get; init; }
}
