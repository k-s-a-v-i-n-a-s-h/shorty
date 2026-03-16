using Shorty.Management.Api.Extensions;
using Shorty.Management.Api.Filters;

namespace Shorty.Management.Api.Features.Shorten;

internal sealed class Shorten : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<ShortenRepository>();
        services.AddScoped<ShortenService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .RequireRateLimiting(SecurityConstants.RateLimitPolicy);

        api.MapPost("/links", ShortenHandler.HandleAsync)
            .AddEndpointFilter<ValidationFilter<ShortenRequest>>();
    }
}
