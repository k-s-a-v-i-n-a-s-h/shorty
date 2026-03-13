using Microsoft.EntityFrameworkCore;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Infrastructure.Persistence;

namespace Shorty.Management.Api.Features.Shorten;

internal sealed class ShortenRepository
{
    private readonly ApplicationDbContext _context;

    public ShortenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> SlugExistsAsync(string slug) =>
        _context.Links.AnyAsync(l => l.Slug == slug);

    public async Task AddLinkAsync(Link link) =>
        await _context.Links.AddAsync(link);
}
