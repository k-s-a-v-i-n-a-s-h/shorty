using Microsoft.IdentityModel.JsonWebTokens;
using Shorty.Management.Domain.Interfaces;
using System.Security.Claims;

namespace Shorty.Management.Api.Services;

internal sealed class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var claimValue = user?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        UserId = Guid.TryParse(claimValue, out var id) ? id : null;

        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;
    }

    //private Guid? _userId;
    //public Guid UserId => _userId ??= GetUserId();

    //private bool? _isAuthenticated;
    //public bool IsAuthenticated => _isAuthenticated ??=
    //    _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    //private Guid GetUserId()
    //{
    //    var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    //    return Guid.TryParse(claimValue, out var id) ? id : Guid.Empty;
    //}
}