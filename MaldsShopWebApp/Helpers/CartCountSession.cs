using MaldsShopWebApp.Interfaces;

namespace MaldsShopWebApp.Helpers
{
    public class CartCountSession
    {
        private readonly IUserRepository _userRepository;

        public CartCountSession(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async void UpdateCount(string userEmail, HttpContext httpContext)
        {
            var user = await _userRepository.GetByEmail(userEmail);
            var cartItems = user.ShippingCart.ShippingCartItems;
            var itemsInTheCart = 0;
            foreach (var item in cartItems)
            {
                itemsInTheCart = itemsInTheCart + item.Quantity;
            }

            httpContext.Session.SetInt32("CartCount", itemsInTheCart);
        }
    }
}