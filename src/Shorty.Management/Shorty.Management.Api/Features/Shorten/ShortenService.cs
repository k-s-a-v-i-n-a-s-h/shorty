using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;

namespace Shorty.Management.Api.Features.Shorten;

internal sealed class ShortenService
{
    private readonly AppOptions _options;
    private readonly ShortenRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ShortenService(IOptions<AppOptions> options, ShortenRepository repository, IUnitOfWork unitOfWork)
    {
        _options = options.Value;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public Task<bool> SlugExistsAsync(string slug) =>
        _repository.SlugExistsAsync(slug);

    public async Task<ShortenResult> AddLinkAsync(string? slug, string url, Guid? userId)
    {
        var link = Link.Create(slug, url, userId);
        await _repository.AddLinkAsync(link);
        await _unitOfWork.SaveChangesAsync();

        var path = $"/{link.Slug}";
        return new(path, _options.GetFrontendUrl(path), link.ExpiresAt);
    }
}
