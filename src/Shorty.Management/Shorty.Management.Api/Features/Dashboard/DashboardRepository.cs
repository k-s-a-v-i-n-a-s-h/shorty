using Microsoft.EntityFrameworkCore;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Infrastructure.Persistence;

namespace Shorty.Management.Api.Features.Dashboard;

internal sealed class DashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Link>> GetLinksAsync(Guid userId) =>
        _context.Links
            .Where(l => l.UserId == userId && l.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public Task<int> DeleteLinkAsync(Guid userId, Guid id) =>
        _context.Links
            .Where(l => l.UserId == userId && l.Id == id)
            .ExecuteDeleteAsync();

    public Task<Link?> GetLinkAsync(Guid userId, Guid id) =>
        _context.Links.FirstOrDefaultAsync(l => l.UserId == userId && l.Id == id);
}
