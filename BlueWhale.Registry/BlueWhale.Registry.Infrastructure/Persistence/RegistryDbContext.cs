using Microsoft.EntityFrameworkCore;
using BlueWhale.Registry.Domain.Entities;

namespace BlueWhale.Registry.Infrastructure.Persistence;

public class RegistryDbContext : DbContext
{
    public RegistryDbContext(DbContextOptions<RegistryDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<RegistrySetting> Settings { get; set; }
    public DbSet<AccessControl> AccessControls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.ActivityLogs)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ResourceType).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ResourceId).HasMaxLength(500);
            entity.HasIndex(e => new { e.UserId, e.Timestamp });
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<RegistrySetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Value).IsRequired();
            entity.HasIndex(e => new { e.Key, e.Category }).IsUnique();
        });

        modelBuilder.Entity<AccessControl>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Repository).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => new { e.UserId, e.Repository });
        });
    }
}
