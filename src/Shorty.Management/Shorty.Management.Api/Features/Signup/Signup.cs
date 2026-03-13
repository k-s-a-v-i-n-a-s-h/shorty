using Shorty.Management.Api.Extensions;
using Shorty.Management.Api.Filters;
using Shorty.Management.Domain.Constants;

namespace Shorty.Management.Api.Features.Signup;

internal sealed class Signup : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<SignupRepository>();
        services.AddScoped<SignupService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .RequireRateLimiting(SecurityConstants.RateLimitPolicy);

        auth.MapPost("/signup", SignupHandler.HandleAsync)
            .AddEndpointFilter<ValidationFilter<SignupRequest>>()
            .AllowAnonymous();

        auth.MapPost("/verify", SignupHandler.VerifyAsync)
            .AddEndpointFilter<ValidationFilter<VerifyRequest>>()
            .AllowAnonymous();

        auth.MapPost("/resend", SignupHandler.ResendAsync)
            .AddEndpointFilter<ValidationFilter<ResendRequest>>()
            .AllowAnonymous();
    }
}
