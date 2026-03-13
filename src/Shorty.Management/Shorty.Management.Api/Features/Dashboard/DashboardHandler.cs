using Shorty.Management.Domain.Interfaces;

namespace Shorty.Management.Api.Features.Dashboard;

internal sealed class DashboardHandler
{
    public static async Task<IResult> GetAsync(ICurrentUserService currentUser, DashboardService service) =>
        Results.Ok(await service.GetDashboardAsync(currentUser.UserId!.Value));

    public static async Task<IResult> DeleteLinkAsync(DashboardService service, ICurrentUserService currentUser, Guid id)
    {
        await service.DeleteLinkAsync(currentUser.UserId!.Value, id);
        return Results.NoContent();
    }

    public static async Task<IResult> UpdateLinkAsync(ICurrentUserService currentUser, DashboardService service, UpdateLinkRequest request, Guid id)
    {
        var link = await service.GetLinkAsync(currentUser.UserId!.Value, id);
        if (link is null)
            return Results.NotFound();

        await service.UpdateLinkAsync(link, request.IsEnabled, request.Url);
        return Results.NoContent();
    }
}
