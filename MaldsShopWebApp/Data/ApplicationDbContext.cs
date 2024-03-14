using MaldsShopWebApp.Models;
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

        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);

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
    public async Task SoftDeleteProductAsync(int productId)
    {
        var product = await Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product != null)
        {
            product.IsDeleted = true;
            await SaveChangesAsync();
        }
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        foreach (var entry in ChangeTracker.Entries<SoftDeleteEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.CurrentValues["IsDeleted"] = false;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["IsDeleted"] = true;
                    break;
            }
        }
    }
}
