using Shorty.Management.Api.Extensions;
using Shorty.Management.Api.Filters;

namespace Shorty.Management.Api.Features.Dashboard;

internal sealed class Dashboard : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<DashboardRepository>();
        services.AddScoped<DashboardService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .RequireAuthorization();

        api.MapGet("/dashboard", DashboardHandler.GetAsync);

        var links = api.MapGroup("/links");

        links.MapDelete("/{id:guid}", DashboardHandler.DeleteLinkAsync);

        links.MapPatch("/{id:guid}", DashboardHandler.UpdateLinkAsync)
            .AddEndpointFilter<ValidationFilter<UpdateLinkRequest>>();
    }
}
