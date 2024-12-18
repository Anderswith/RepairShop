using Microsoft.EntityFrameworkCore;
using RepairShop.BE;

namespace RepairShop.DAL;

public class RepairShopContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<UserData> UserData { get; set;}


    public RepairShopContext(DbContextOptions<RepairShopContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        base.OnModelCreating(modelBuilder);
        

    }
    
}

