using FluentValidation;

namespace Shorty.Management.Api.Features.Login;

internal sealed record LoginRequest
{
    public required string Email { get => field; init => field = value.Trim().ToLowerInvariant(); }
    public required string Password { get; init; }
}

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(64);
    }
}