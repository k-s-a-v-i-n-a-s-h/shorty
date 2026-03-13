using FluentValidation;
using Shorty.Management.Domain.Interfaces;

namespace Shorty.Management.Api.Features.Shorten;

internal sealed record ShortenRequest
{
    public required string Url { get; init => field = value.Trim(); }
    public string? Alias { get; init => field = value?.Trim().ToLowerInvariant(); }
}

internal sealed record ShortenResponse(
    string ShortUrl,
    DateTime ExpiresAt
);

internal sealed class ShortenRequestValidator : AbstractValidator<ShortenRequest>
{
    public ShortenRequestValidator(ICurrentUserService currentUser)
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .MaximumLength(2048)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid URL");

        When(r => r.Alias is not null, () =>
        {
            RuleFor(x => x.Alias)
                .Must(_ => currentUser.IsAuthenticated)
                .WithMessage("You must be logged in to specify an alias")
                .NotEmpty()
                .MaximumLength(255)
                .Matches(@"^[a-z0-9\-]+$")
                .WithMessage("Can only contain letters, numbers, and dashes");
        });
    }
}