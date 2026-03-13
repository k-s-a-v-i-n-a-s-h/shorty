using System.ComponentModel.DataAnnotations;

namespace Shorty.Management.Worker.Options;

internal sealed class SmtpOptions
{
    [Required]
    public required string Host { get; init; }

    [Required, Range(1, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
    public required int Port { get; init; }

    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }
}
