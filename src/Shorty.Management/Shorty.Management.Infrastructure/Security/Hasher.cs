using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Shorty.Management.Infrastructure.Security;

public sealed class Hasher : IHasher
{
    private const int _iterations = 600000;
    private const int _keyLength = 32;
    private const int _saltSize = 16;
    private readonly byte[] _key;

    public Hasher(IOptions<HasherOptions> options)
    {
        _key = Encoding.UTF8.GetBytes(options.Value.Secret);
    }

    public static (byte[] Hash, byte[] Salt) Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(_saltSize);

        var hash = KeyDerivation.Pbkdf2(
            password: value,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: _iterations,
            numBytesRequested: _keyLength
        );

        return (hash, salt);
    }

    public static bool Verify(string value, byte[] hash, byte[] salt)
    {
        var inputHash = KeyDerivation.Pbkdf2(
            password: value,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: _iterations,
            numBytesRequested: _keyLength
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    public string Hash(Guid value)
    {
        var payload = value.ToByteArray();

        using var hmac = new HMACSHA256(_key);
        var hashBytes = hmac.ComputeHash(payload);

        return Base64Url.EncodeToString(hashBytes);
    }

    public bool Verify(Guid value, string hash)
    {
        var computedHash = Hash(value);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hash),
            Encoding.UTF8.GetBytes(computedHash)
        );
    }
}