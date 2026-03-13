using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Constants;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Enums;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;
using Shorty.Management.Infrastructure.Security;
using System.Net;

namespace Shorty.Management.Api.Features.Signup;

internal sealed class SignupService
{
    private readonly IHasher _hasher;
    private readonly SignupRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppOptions _appOptions;

    public SignupService(IOptions<AppOptions> appOptions, IHasher hasher, SignupRepository repository, IUnitOfWork unitOfWork)
    {
        _hasher = hasher;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _appOptions = appOptions.Value;
    }

    public Task<bool> UserExistsAsync(string email) =>
        _repository.UserExistsAsync(email);

    public async Task AddUserAsync(string email, string password)
    {
        var (hash, salt) = Hasher.Hash(password);
        var user = User.Create(email, hash, salt);

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _repository.AddUserAsync(user);
            await EnqueueVerificationEmailAsync(user);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task EnqueueVerificationEmailAsync(User user)
    {
        var template = await _repository.GetTemplateAsync(TemplateType.VerifyEmail)
            ?? throw new KeyNotFoundException($"Template for type '{TemplateType.VerifyEmail}' was not found in the database.");

        var path = $"{FrontendRoutes.VerifyEmail}/{user.Id}";
        var url = $"{_appOptions.GetFrontendUrl(path)}?token={_hasher.Hash(user.Id)}";

        var renderedBody = template.HtmlBody.Replace("{{VerificationUrl}}", WebUtility.HtmlEncode(url));

        var email = Outbox.Create(TemplateType.VerifyEmail, user.Email, template.Subject, renderedBody);
        await _repository.AddOutboxAsync(email);
        await _unitOfWork.SaveChangesAsync();
    }

    public Task<User?> GetUserAsync(Guid userId) =>
        _repository.GetUserAsync(userId);

    public async Task MarkAsVerifiedAsync(User user)
    {
        user.IsVerified = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public Task<User?> GetUserAsync(string email) =>
        _repository.GetUserAsync(email);
}
