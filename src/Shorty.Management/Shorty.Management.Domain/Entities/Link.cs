using Shorty.Management.Domain.Enums;
using System.Buffers.Text;

namespace Shorty.Management.Domain.Entities;

public class Link
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Slug { get; set; }
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public Guid? UserId { get; set; } //Foreign key
    public LinkStatus Status { get; set; } = LinkStatus.Active;
    public int TotalClicks { get; set; } = 0;
    public virtual User User { get; set; } = default!;
    public static Link Create(string? slug, string url, Guid? userId)
    {
        var id = Guid.CreateVersion7();
        slug ??= Base64Url.EncodeToString(id.ToByteArray());
        var expiryDays = userId is not null ? 7 : 1;

        return new Link
        {
            Id = id,
            Slug = slug,
            Url = url,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
        };
    }
    public void UpdateStatus(bool isEnabled) => Status = isEnabled ? LinkStatus.Active : LinkStatus.Disabled;
    public void UpdateUrl(string url) => Url = url;
}