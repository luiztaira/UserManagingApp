using Microsoft.EntityFrameworkCore;
using UserManagingApp.Models;

public class UserManagerContext : DbContext
{
    public DbSet<User> Users { get; set; }

    //sql database コネクション文字列
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userManager;Username=postgres;Password=luiz2803");
    }
    //模擬データを作る
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Luiz Taira", Email = "luiztaira@example.com" },
            new User { Id = 2, Name = "Yagami Taichi", Email = "yagamitaichi@example.com" }
        );
    }
}
