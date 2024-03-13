using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaldsShopWebApp.Controllers
{
    public class ShippingCartController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IShippingCartRepository _shippingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly CartCountSession _cartCountSession;

        public ShippingCartController(
            IUserRepository userRepository, 
            IShippingCartRepository shippingCartRepository, 
            IProductRepository productRepository,
            CartCountSession cartCountSession
            )
        {
            _userRepository = userRepository;
            _shippingCartRepository = shippingCartRepository;
            _productRepository = productRepository;
            _cartCountSession = cartCountSession;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var user = await _userRepository.GetByEmail(userEmail);
            var shippingCartVM = new ShippingCartViewModel()
            {
                ShippingCart = user.ShippingCart,
                UserEmail = userEmail
            };

            //_cartCountSession.UpdateCount(User.Identity.Name, HttpContext);

            var cartItems = user.ShippingCart.ShippingCartItems;
            var itemsInTheCart = 0;
            foreach (var item in cartItems)
            {
                itemsInTheCart = itemsInTheCart + item.Quantity;
            }

            HttpContext.Session.SetInt32("CartCount", itemsInTheCart);

            return View(shippingCartVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, bool isRedirected)
        {
            var userEmail = User.Identity.Name;
            var user = await _userRepository.GetByEmail(userEmail);
            var product = await _productRepository.GetByIdAsync(productId);

            if (user != null && product != null)
            {
                var shippingCart = await _shippingCartRepository.GetShippingCartByUserEmail(userEmail);

                if (shippingCart == null)
                {
                    shippingCart = new ShippingCart
                    {
                        AppUserId = user.Id,
                        ShippingCartItems = new List<ShippingCartItem>(),
                    };
                    await _shippingCartRepository.Add(shippingCart);
                }

                var cartItem = shippingCart.ShippingCartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += 1;
                }
                else
                {
                    cartItem = new ShippingCartItem
                    {
                        Quantity = 1,
                        ProductId = productId,
                        ShippingCartId = shippingCart.ShippingCartId
                    };
                    shippingCart.ShippingCartItems.Add(cartItem);
                }

                await _shippingCartRepository.SaveAsync();

                var updatedUser = await _userRepository.GetByEmail(userEmail);

                var cartVM = new ShippingCartViewModel
                {
                    UserEmail = userEmail,
                    ShippingCart = updatedUser.ShippingCart
                };

                if (isRedirected)
                {
                    View("Index", cartVM);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    View("Index", cartVM);
                    return RedirectToAction("Index", "ShippingCart");
                }
            }
            return View("Error");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId, bool isRedirected)
        {
            var userEmail = User.Identity.Name;
            var user = await _userRepository.GetByEmail(userEmail);
            var product = await _productRepository.GetByIdAsync(productId);

            if (user != null && product != null)
            {
                var shippingCart = await _shippingCartRepository.GetShippingCartByUserEmail(userEmail);
                var cartItem = shippingCart.ShippingCartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity -= 1;
                    if (cartItem.Quantity <= 0)
                    {
                        shippingCart.ShippingCartItems.Remove(cartItem);
                    }

                    await _shippingCartRepository.SaveAsync();

                    var updatedUser = await _userRepository.GetByEmail(userEmail);
                    var cartVM = new ShippingCartViewModel
                    {
                        UserEmail = userEmail,
                        ShippingCart = updatedUser.ShippingCart
                    };
                    if (isRedirected)
                    {
                        View("Index", cartVM);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        View("Index", cartVM);
                        return RedirectToAction("Index", "ShippingCart");
                    }
                }
                else
                {
                    return View("Error", new ErrorViewModel { RequestId = "Product not found in cart" });
                }
            }
            return View("Error", new ErrorViewModel { RequestId = "User or Product not found" });
        }
    }
}
