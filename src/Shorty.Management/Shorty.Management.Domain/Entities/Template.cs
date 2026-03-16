using Shorty.Management.Domain.Enums;

namespace Shorty.Management.Domain.Entities;

public class Template
{
    public required TemplateType Type { get; set; }
    public required string HtmlBody { get; set; }
    public required string Subject { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
