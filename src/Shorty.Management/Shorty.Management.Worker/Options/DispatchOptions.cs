using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Worker.Options;

internal sealed class DispatchOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "{0} must be at least {1} second.")]
    public required int ErrorBackoffSeconds { get; init; }

    [Required, Range(1, 4, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int MaxConcurrency { get; init; }
}
