using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Name).IsRequired().HasMaxLength(255).HasColumnType("citext");
            entity.HasIndex(o => o.Name).IsUnique().HasFilter("\"IsDeleted\" = false");

            entity.HasQueryFilter(o => !o.IsDeleted);
        });
    }
}
