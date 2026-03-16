using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Domain.Options;

public sealed class MessagingOptions
{
    [Required]
    public required Dictionary<string, Subject> Subjects { get; init; }

    [Required, ValidateObjectMembers]
    public required Dictionary<string, StreamOptions> Streams { get; init; }

    [Required, ValidateObjectMembers]
    public required Dictionary<string, ConsumerOptions> Consumers { get; init; }
}

public sealed class Subject
{
    [Required]
    public required string Stream { get; init; }

    [Required]
    public required string Value { get; init; }
}

public sealed class StreamOptions
{
    [Required]
    public required string[] Subjects { get; init; }

    [Required, Range(1, long.MaxValue, ErrorMessage = "{0} must be atleast {1} bytes.")]
    public required long MaxBytes { get; init; }

    [Required, Range(1, 365, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int MaxAgeDays { get; init; }
}

public sealed class ConsumerOptions
{
    [Required]
    public required string Stream { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    public required string[] Subjects { get; init; }

    [Required, Range(1, 32, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int MaxAckPending { get; init; }

    [Required, Range(1, 100, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int MaxDeliver { get; init; }

    [Required, Range(1, 3600, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int AckWaitSeconds { get; init; }
}