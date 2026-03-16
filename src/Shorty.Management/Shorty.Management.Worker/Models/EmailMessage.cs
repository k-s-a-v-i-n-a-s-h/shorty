using System.Text.Json;

namespace Shorty.Management.Worker.Models;

internal sealed record EmailMessage
{
    public required Sender From { get; init; }
    public required string To { get; init; }
    public string[] Cc { get; init; } = [];
    public string[] Bcc { get; init; } = [];
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public override string ToString() => JsonSerializer.Serialize(this);
};

internal sealed record Sender(string Name, string Email);