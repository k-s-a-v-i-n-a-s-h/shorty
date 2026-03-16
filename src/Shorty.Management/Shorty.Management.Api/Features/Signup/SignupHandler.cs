using Microsoft.Extensions.Options;
using Shorty.Management.Api.Extensions;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;

namespace Shorty.Management.Api.Features.Signup;

internal static class SignupHandler
{
    public static async Task<IResult> HandleAsync(SignupService service, SignupRequest request)
    {
        if (await service.UserExistsAsync(request.Email))
            return Results.Problem(statusCode: StatusCodes.Status409Conflict, detail: "Email already in use");

        await service.AddUserAsync(request.Email, request.Password);
        return Results.Created();
    }

    public static async Task<IResult> VerifyAsync(HttpContext context, IOptions<ExpiryOptions> options, IJwtProvider jwtProvider, SignupService service, VerifyRequest request)
    {
        var user = await service.GetUserAsync(request.UserId);
        if (user is null)
            return Results.Problem(statusCode: StatusCodes.Status404NotFound, detail: "Invalid account");

        if (user.IsVerified)
            return Results.Problem(statusCode: StatusCodes.Status409Conflict, detail: "Email already verified");

        await service.MarkAsVerifiedAsync(user);

        var expiresAt = DateTime.UtcNow.AddDays(options.Value.LoginDays);
        var token = jwtProvider.Generate(user.Id, expiresAt);
        context.Response.AppendAuthCookie(token, expiresAt);

        return Results.Ok();
    }

    public static async Task<IResult> ResendAsync(SignupService service, ResendRequest request)
    {
        var user = await service.GetUserAsync(request.Email);
        if (user is null)
            return Results.Problem(statusCode: StatusCodes.Status404NotFound, detail: "Email not found");

        if (user.IsVerified)
            return Results.Problem(statusCode: StatusCodes.Status409Conflict, detail: "Email already verified");

        await service.EnqueueVerificationEmailAsync(user);
        return Results.Accepted();
    }
}
