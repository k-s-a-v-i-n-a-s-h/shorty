namespace Shorty.Management.Domain.Interfaces;

public interface IHasher
{
    string Hash(Guid value);
    bool Verify(Guid value, string hash);
}
