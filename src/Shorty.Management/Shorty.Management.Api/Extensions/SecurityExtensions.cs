using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shorty.Management.Domain.Constants;
using Shorty.Management.Infrastructure.Options;
using System.Text;
using System.Threading.RateLimiting;

namespace Shorty.Management.Api.Extensions;

internal static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddForwardedHeaders();
        services.AddIpRateLimiting();
        services.AddJwtAuth();

        return services;
    }

    private static IServiceCollection AddForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }

    private static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, settings) =>
            {
                options.MapInboundClaims = false;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[SecurityConstants.CookieName];
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Value.Issuer,
                    ValidAudience = settings.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.Key))
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddIpRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter();

        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitOptions>>((options, settings) =>
            {
                options.AddPolicy(SecurityConstants.RateLimitPolicy, context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIp,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = settings.Value.PermitLimit,
                            Window = TimeSpan.FromMinutes(settings.Value.WindowMinutes),
                            QueueLimit = 0,
                        });
                });

                options.OnRejected = (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.Headers.RetryAfter = (settings.Value.WindowMinutes * 60).ToString();
                    return ValueTask.CompletedTask;
                };
            });

        return services;
    }
}
