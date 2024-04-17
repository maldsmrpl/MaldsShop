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
        public async Task AddAsync(ShippingCart shippingCart)
        {
            _context.ShippingCarts.AddAsync(shippingCart);
        }
        public async Task UpdateAsync(ShippingCart shippingCart)
        {
            _context.ShippingCarts.Update(shippingCart);
        }
        public async Task AddToShippingCartAsync(ShippingCartItem item, string userEmail)
        {
            var shippingCart = await GetShippingCartByUserEmail(userEmail);
            var currentUser = await _userRepository.GetByEmailAsync(userEmail);

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
        }
        public async Task DeleteFromShippingCart(ShippingCartItem item, string userEmail)
        {
            var shippingCart = await GetShippingCartByUserEmail(userEmail);
            if (shippingCart != null)
            {
                shippingCart.ShippingCartItems.FirstOrDefault(i => i.ShippingCartItemId == item.ShippingCartItemId);
            }
        }
        public async Task ClearShippingCart(ShippingCart shippingCart)
        {
            shippingCart.ShippingCartItems.Clear();
            await UpdateAsync(shippingCart);
        }
        public async Task<ShippingCart>? GetShippingCartByUserEmail(string userEmail)
        {
            var user = await _userRepository.GetByEmailAsync(userEmail);
            return await _context.ShippingCarts
                .Include(i => i.ShippingCartItems)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(e => e.AppUserId == user.Id);
        }
    }
}
