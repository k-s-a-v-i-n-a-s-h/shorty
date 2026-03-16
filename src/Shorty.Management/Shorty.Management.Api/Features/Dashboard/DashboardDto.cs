using FluentValidation;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Enums;
using Shorty.Management.Infrastructure.Options;

namespace Shorty.Management.Api.Features.Dashboard;

internal sealed record DashboardResponse(
    IEnumerable<LinkDto> Links
);

internal sealed record LinkDto(
    Guid Id,
    string Url,
    string Alias,
    string ShortUrl,
    LinkStatus Status,
    int TotalClicks,
    DateTime CreatedAt,
    DateTime ExpiresAt
);

internal static class DashboardDtoExtensions
{
    public static LinkDto ToDto(this Link link, AppOptions options)
    {
        return new(
            link.Id,
            link.Url,
            link.Slug,
            options.GetFrontendUrl($"/{link.Slug}"),
            link.Status,
            link.TotalClicks,
            link.CreatedAt,
            link.ExpiresAt
        );
    }
}

internal sealed record UpdateLinkRequest(
    bool? IsEnabled,
    string? Url
);

internal sealed class UpdateLinkRequestValidator : AbstractValidator<UpdateLinkRequest>
{
    public UpdateLinkRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Url is not null || x.IsEnabled.HasValue)
            .WithMessage("Provide at least one field to update");

        When(r => r.Url is not null, () =>
        {
            RuleFor(x => x.Url)
                .NotEmpty()
                .MaximumLength(2048)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid URL");
        });
    }
}
