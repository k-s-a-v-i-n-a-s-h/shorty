using FluentValidation;
using Shorty.Management.Domain.Interfaces;
using System.Buffers.Text;

namespace Shorty.Management.Api.Features.Signup;

internal sealed record SignupRequest
{
    public required string Email { get => field; init => field = value.Trim().ToLowerInvariant(); }
    public required string Password { get; init; }
}

internal sealed class SignupRequestValidator : AbstractValidator<SignupRequest>
{
    public SignupRequestValidator()
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

internal sealed record VerifyRequest
{
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
}

internal sealed class VerifyRequestValidator : AbstractValidator<VerifyRequest>
{
    private readonly IHasher _hasher;

    public VerifyRequestValidator(IHasher hasher)
    {
        _hasher = hasher;

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty()
            .Must(token => Base64Url.IsValid(token))
            .WithMessage("The token is not in a valid format.")
            .Must((r, token) => _hasher.Verify(r.UserId, token))
            .WithMessage("The token is invalid for this user.");
    }
}


internal sealed record ResendRequest
{
    public required string Email { get => field; init => field = value.Trim().ToLowerInvariant(); }
}

internal sealed class ResendRequestValidator : AbstractValidator<ResendRequest>
{
    public ResendRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
    }
}
