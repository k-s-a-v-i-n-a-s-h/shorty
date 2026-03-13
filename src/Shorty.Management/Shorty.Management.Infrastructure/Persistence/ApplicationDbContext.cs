using Microsoft.EntityFrameworkCore;
using Shorty.Management.Domain.Entities;
using Shorty.Management.Domain.Interfaces;

namespace Shorty.Management.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<Outbox> Outbox { get; set; }
    public DbSet<Template> Templates { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (Database.CurrentTransaction is null)
            await Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await SaveChangesAsync();

        if (Database.CurrentTransaction is not null)
            await Database.CurrentTransaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (Database.CurrentTransaction is not null)
            await Database.CurrentTransaction.RollbackAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email).HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PasswordHash).HasMaxLength(32);

            entity.Property(u => u.PasswordSalt).HasMaxLength(16);

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Link>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Slug).IsUnique();

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(u => u.ExpiresAt);

            entity.HasOne(l => l.User).WithMany(u => u.Links).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Outbox>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasOne(o => o.Template).WithMany().HasForeignKey(o => o.Type).OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.Type).HasConversion<string>();

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(u => u.ProcessedAt).HasFilter("\"ProcessedAt\" IS NULL");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(u => u.Type);
            entity.Property(u => u.Type).HasConversion<string>();

            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}