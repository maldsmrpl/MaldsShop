using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ShippingCart> ShippingCarts { get; set; }
    public DbSet<ShippingCartItem> ShippingCartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.ShippingCart)
            .WithOne(c => c.AppUser)
            .HasForeignKey<ShippingCart>(c => c.AppUserId);

        modelBuilder.Entity<OrderItem>()
            .HasQueryFilter(oi => !oi.Product.IsDeleted);

        modelBuilder.Entity<ShippingCartItem>()
            .HasQueryFilter(sci => !sci.Product.IsDeleted);
    }
}
