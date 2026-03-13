using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Infrastructure.Options;

public sealed class ExpiryOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "Login expiry must be at least 1 day.")]
    public required int LoginDays { get; init; }
}
