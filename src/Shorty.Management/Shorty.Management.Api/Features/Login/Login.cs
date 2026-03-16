using Shorty.Management.Api.Extensions;
using Shorty.Management.Api.Filters;

namespace Shorty.Management.Api.Features.Login;

internal sealed class Login : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<LoginRepository>();
        services.AddScoped<LoginService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .RequireRateLimiting(SecurityConstants.RateLimitPolicy);

        auth.MapPost("/login", LoginHandler.HandleAsync)
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .AllowAnonymous();
    }
}
