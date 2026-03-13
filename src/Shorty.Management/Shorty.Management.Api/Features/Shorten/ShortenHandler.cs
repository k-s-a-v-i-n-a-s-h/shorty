using Shorty.Management.Domain.Interfaces;

namespace Shorty.Management.Api.Features.Shorten;

internal static class ShortenHandler
{
    public static async Task<IResult> HandleAsync(ICurrentUserService currentUser, ShortenService service, ShortenRequest request)
    {
        if (request.Alias is not null && await service.SlugExistsAsync(request.Alias))
            return Results.Problem(statusCode: StatusCodes.Status409Conflict, detail: "Alias already taken");

        var result = await service.AddLinkAsync(request.Alias, request.Url, currentUser.UserId);
        return Results.Created(result.Path, result.ToDto());
    }
}
