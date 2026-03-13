using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shorty.Management.Infrastructure.Options;
using System.Security.Claims;
using System.Text;

namespace Shorty.Management.Infrastructure.Security;

public interface IJwtProvider
{
    string Generate(Guid userId, DateTime expiresAt);
}

internal sealed class JwtProvider : IJwtProvider
{
    private static readonly JsonWebTokenHandler _handler = new();
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(Guid userId, DateTime expiresAt)
    {
        var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        return _handler.CreateToken(descriptor);
    }
}
