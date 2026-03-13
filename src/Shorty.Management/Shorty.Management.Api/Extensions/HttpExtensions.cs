using Shorty.Management.Domain.Constants;

namespace Shorty.Management.Api.Extensions;

internal static class HttpExtensions
{
    public static void AppendAuthCookie(this HttpResponse response, string token, DateTimeOffset expiresAt)
    {
        response.Cookies.Append(SecurityConstants.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = SecurityConstants.CookiePath
        });
    }

    public static void DeleteAuthCookie(this HttpResponse response)
    {
        response.Cookies.Delete(SecurityConstants.CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = SecurityConstants.CookiePath
        });
    }
}
