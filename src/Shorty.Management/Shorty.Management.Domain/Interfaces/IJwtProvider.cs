namespace Shorty.Management.Domain.Interfaces;

public interface IJwtProvider
{
    string Generate(Guid userId, DateTime expiresAt);
}
