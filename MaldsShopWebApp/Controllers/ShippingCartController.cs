using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaldsShopWebApp.Controllers
{
    public class ShippingCartController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IShippingCartRepository _shippingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public ShippingCartController(IUserRepository userRepository, IShippingCartRepository shippingCartRepository, IProductRepository productRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _shippingCartRepository = shippingCartRepository;
            _productRepository = productRepository;
            _context = context;
        }
        public IActionResult Index()
        {
            var userEmail = User.Identity.Name;
            var user = _userRepository.GetByEmail(userEmail);
            var shippingCartVM = new ShippingCartViewModel()
            {
                ShippingCart = _shippingCartRepository.GetShippingCartByUserEmail(userEmail).Result,
                UserEmail = userEmail
            };
            return View(shippingCartVM);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var userEmail = User.Identity.Name;
            var user = _userRepository.GetByEmail(userEmail).Result;
            var product = _productRepository.GetByIdAsync(productId).Result;
            _context.Products.Attach(product);

            if (user != null && product != null)
            {
                var cartItem = new ShippingCartItem()
                {
                    Quantity = 1,
                    ProductId = product.ProductId
                };
                _shippingCartRepository.AddToShippingCart(cartItem, userEmail);
                var cartVM = new ShippingCartViewModel()
                {
                    UserEmail = userEmail,
                    ShippingCart = _shippingCartRepository.GetShippingCartByUserEmail(userEmail).Result
                };
                return View("Index");
            }
            return View("Error");
        }
    }
}
