using Microsoft.EntityFrameworkCore;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Infrastructure.Persistence;

namespace Shorty.Management.Api.Features.Login;

internal sealed class LoginRepository
{
    private readonly ApplicationDbContext _context;

    public LoginRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
