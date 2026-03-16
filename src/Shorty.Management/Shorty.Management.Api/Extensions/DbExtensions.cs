using Microsoft.EntityFrameworkCore;
using Shorty.Management.Infrastructure.Persistence;

namespace Shorty.Management.Api.Extensions;

internal static class DbExtensions
{
    public static IApplicationBuilder InitDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;");

        return app;
    }
}
