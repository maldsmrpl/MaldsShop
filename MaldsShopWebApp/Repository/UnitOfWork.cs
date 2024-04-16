using MaldsShopWebApp.Interfaces;

namespace MaldsShopWebApp.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IUserRepository Users { get; private set; }
        public IShippingCartRepository ShippingCarts { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Orders = new OrderRepository(_context);
            Products = new ProductRepository(_context);
            Reviews = new ReviewRepository(_context);
            Users = new UserRepository(_context);
            ShippingCarts = new ShippingCartRepository(_context, Users);
        }
        public async Task<bool> CompleteAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
