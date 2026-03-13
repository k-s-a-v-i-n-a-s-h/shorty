using Shorty.Management.Domain.Enums;

namespace Shorty.Management.Domain.Entities;

public class Outbox
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required TemplateType Type { get; set; }
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string HtmlBody { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? LastError { get; set; }
    public virtual Template Template { get; set; } = default!;
    public static Outbox Create(TemplateType type, string recipient, string subject, string htmlBody)
    {
        return new Outbox
        {
            Recipient = recipient,
            Type = type,
            Subject = subject,
            HtmlBody = htmlBody
        };
    }
    public void Processed() => ProcessedAt = DateTime.UtcNow;
    public void Error(string message) => LastError = message;
}