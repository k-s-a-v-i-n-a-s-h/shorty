using Microsoft.Extensions.Options;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Interfaces;
using Shorty.Management.Infrastructure.Options;

namespace Shorty.Management.Api.Features.Dashboard;

internal sealed class DashboardService
{
    private readonly AppOptions _options;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DashboardRepository _repository;

    public DashboardService(IUnitOfWork unitOfWork, IOptions<AppOptions> options, DashboardRepository repository)
    {
        _options = options.Value;
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(Guid userId)
    {
        var links = await _repository.GetLinksAsync(userId);

        return new(
            links.Select(l => l.ToDto(_options))
        );
    }

    public Task<int> DeleteLinkAsync(Guid userId, Guid id) =>
        _repository.DeleteLinkAsync(userId, id);

    public Task<Link?> GetLinkAsync(Guid userId, Guid id) =>
        _repository.GetLinkAsync(userId, id);

    public async Task UpdateLinkAsync(Link link, bool? isEnabled, string? url)
    {
        if (isEnabled.HasValue)
            link.UpdateStatus(isEnabled.Value);

        if (url is not null)
            link.UpdateUrl(url);

        await _unitOfWork.SaveChangesAsync();
    }
}
