using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Worker.Options;

internal sealed class CleanupOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} hour.")]
    public required int PollingIntervalHours { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} day.")]
    public required int UserRetentionDays { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} day.")]
    public required int OutboxRetentionDays { get; init; }
}
