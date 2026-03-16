using Microsoft.EntityFrameworkCore;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Enums;
using Shorty.Management.Infrastructure.Persistence;

namespace Shorty.Management.Api.Features.Signup;

internal sealed class SignupRepository
{
    private readonly ApplicationDbContext _context;

    public SignupRepository(ApplicationDbContext context) =>
        _context = context;

    public Task<bool> UserExistsAsync(string email) =>
        _context.Users.AnyAsync(u => u.Email == email);

    public Task<User?> GetUserAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetUserAsync(string email) =>
        _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddUserAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task AddOutboxAsync(Outbox outbox) =>
        await _context.Outbox.AddAsync(outbox);

    public Task<Template?> GetTemplateAsync(TemplateType type) =>
        _context.Templates.AsNoTracking().FirstOrDefaultAsync(t => t.Type == type);
}
