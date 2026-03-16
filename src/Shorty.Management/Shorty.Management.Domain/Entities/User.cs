namespace Shorty.Management.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Email { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsVerified { get; set; }
    public virtual ICollection<Link> Links { get; set; } = [];
    public static User Create(string email, byte[] pHash, byte[] pSalt)
    {
        return new User
        {
            Email = email,
            PasswordHash = pHash,
            PasswordSalt = pSalt,
        };
    }
}