using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Name).IsRequired().HasColumnType("citext");
            entity.HasIndex(o => o.Name).IsUnique().HasFilter("\"IsDeleted\" = false");

            entity.HasQueryFilter(o => !o.IsDeleted);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity
                .HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.Email).IsRequired().HasColumnType("citext");
            entity
                .HasIndex(u => new { u.OrganizationId, u.Email })
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");

            entity.Property(u => u.PasswordHash).IsRequired();

            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(20);

            entity.HasQueryFilter(u => !u.IsDeleted);
        });
    }
}
