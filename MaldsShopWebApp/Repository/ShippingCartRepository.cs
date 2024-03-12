using MaldsShopWebApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaldsShopWebApp.Repository
{
    public class ShippingCartRepository : IShippingCartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public ShippingCartRepository(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }
        public async Task<ShippingCart> GetShippingCartByUserEmail(string userEmail)
        {
            var user = await _userRepository.GetByEmail(userEmail);
            return await _context.ShippingCarts
                .Include(i => i.ShippingCartItems)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(e => e.AppUserId == user.Id);
        }
        public async Task<bool> AddToShippingCart(ShippingCartItem item, string userEmail)
        {
            var shippingCart = await GetShippingCartByUserEmail(userEmail);
            var currentUser = await _userRepository.GetByEmail(userEmail);

            if (shippingCart == null)
            {
                shippingCart = new ShippingCart
                {
                    AppUserId = currentUser.Id,
                    ShippingCartItems = new List<ShippingCartItem>()
                };
                _context.ShippingCarts.Add(shippingCart);
            }

            _context.Products.Attach(item.Product);
            shippingCart.ShippingCartItems.Add(item);

            return await SaveAsync();
        }

        public async Task<bool> DeleteFromShippingCart(ShippingCartItem item, string userEmail)
        {
            var shippingCart = await GetShippingCartByUserEmail(userEmail);
            if (shippingCart != null)
            {
                shippingCart.ShippingCartItems.FirstOrDefault(i => i.ShippingCartItemId == item.ShippingCartItemId);
            }
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool Add(ShippingCart shippingCart)
        {
            _context.ShippingCarts.Add(shippingCart);
            return Save();
        }
        public async Task<bool> SaveAsync()
        {
            try
            {
                var saved = await _context.SaveChangesAsync();
                return saved > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

    }
}
