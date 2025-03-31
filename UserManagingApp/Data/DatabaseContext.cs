using Microsoft.EntityFrameworkCore;
using UserManagingApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

public class UserManagerContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=userManager;Username=postgres;Password=luiz2803");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure timestamp defaults
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.LastUpdatedAt)
                  .HasDefaultValueSql("NOW()")
                  .ValueGeneratedOnAddOrUpdate();
        });

        // Seed data with proper UTC timestamps
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Luiz Taira",
                Email = "luiztaira@example.com",
                PhoneNumber = "+1234567890",
                Privileges = "admin",
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                LastUpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                Name = "Yagami Taichi",
                Email = "yagamitaichi@example.com",
                PhoneNumber = "+9876543210",
                Privileges = "user",
                CreatedAt = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                LastUpdatedAt = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<User>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            entry.Entity.LastUpdatedAt = DateTime.UtcNow;
        }
    }
}