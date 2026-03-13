using Microsoft.Extensions.Options;
using Shorty.Management.Api.Extensions;
using Shorty.Management.Infrastructure.Options;
using Shorty.Management.Infrastructure.Security;

namespace Shorty.Management.Api.Features.Login;

internal static class LoginHandler
{
    public static async Task<IResult> HandleAsync(IOptions<ExpiryOptions> options, HttpContext context, IJwtProvider jwtProvider, LoginService service, LoginRequest request)
    {
        var user = await service.GetUserAsync(request.Email);
        if (user is null)
            return Results.Problem(statusCode: StatusCodes.Status404NotFound, detail: "Email not found");

        if (!Hasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Incorrect password");

        if (!user.IsVerified)
            return Results.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "Email not verified");

        var expiresAt = DateTime.UtcNow.AddDays(options.Value.LoginDays);
        var token = jwtProvider.Generate(user.Id, expiresAt);
        context.Response.AppendAuthCookie(token, expiresAt);

        return Results.Ok();
    }
}
