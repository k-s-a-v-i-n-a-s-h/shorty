using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Worker.Options;

internal sealed class RelayOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1}.")]
    public required int BatchSize { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} seconds.")]
    public required int PollingIntervalSeconds { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} seconds.")]
    public required int ErrorBackoffSeconds { get; init; }

    [Required]
    public required string SenderEmail { get; init; }

    [Required]
    public required string SenderName { get; init; }

    [Required, Range(1, 4, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int MaxConcurrency { get; init; }
}
