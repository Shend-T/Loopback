using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<InviteUser> InviteUsers => Set<InviteUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Issue> Issues => Set<Issue>();

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

        modelBuilder.Entity<InviteUser>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Email).IsRequired().HasColumnType("citext");
            entity.Property(i => i.TokenHash).IsRequired();

            entity.HasIndex(i => i.TokenHash).IsUnique();

            entity
                .HasIndex(i => new { i.OrganizationId, i.Email })
                .IsUnique()
                .HasFilter("\"AcceptedAt\" IS NULL");

            entity
                .HasOne(i => i.Organization)
                .WithMany()
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(i => i.InvitedBy)
                .WithMany()
                .HasForeignKey(i => i.InvitedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(t => t.TokenHash).IsRequired();

            entity.HasIndex(t => t.TokenHash).IsUnique();

            entity.HasQueryFilter(t => !t.User.IsDeleted);
        });

        modelBuilder.Entity<Issue>(e =>
        {
            e.HasOne(i => i.Organization)
                .WithMany()
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(i => i.ReportedBy)
                .WithMany(u => u.ReportedIssues)
                .HasForeignKey(i => i.ReportedById)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(i => i.AssignedTo)
                .WithMany(u => u.AssignedIssues)
                .HasForeignKey(i => i.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasQueryFilter(i => !i.IsDeleted);
            e.Property(i => i.Title).HasMaxLength(200);
            e.HasIndex(i => new { i.OrganizationId, i.Status });
        });
    }
}
