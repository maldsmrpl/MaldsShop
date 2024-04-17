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
        private readonly IUnitOfWork _unitOfWork;
        private readonly CartCountSession _cartCountSession;

        public ShippingCartController(
            IUnitOfWork unitOfWork,
            CartCountSession cartCountSession
            )
        {
            _unitOfWork = unitOfWork;
            _cartCountSession = cartCountSession;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
            var shippingCartVM = new ShippingCartViewModel()
            {
                ShippingCart = user.ShippingCart,
                UserEmail = userEmail
            };

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
            var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (user != null && product != null)
            {
                var shippingCart = await _unitOfWork.ShippingCarts.GetShippingCartByUserEmail(userEmail);

                if (shippingCart == null)
                {
                    shippingCart = new ShippingCart
                    {
                        AppUserId = user.Id,
                        ShippingCartItems = new List<ShippingCartItem>(),
                    };
                    await _unitOfWork.ShippingCarts.AddAsync(shippingCart);
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

                await _unitOfWork.CompleteAsync();

                var updatedUser = await _unitOfWork.Users.GetByEmailAsync(userEmail);

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
            var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (user != null && product != null)
            {
                var shippingCart = await _unitOfWork.ShippingCarts.GetShippingCartByUserEmail(userEmail);
                var cartItem = shippingCart.ShippingCartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity -= 1;
                    if (cartItem.Quantity <= 0)
                    {
                        shippingCart.ShippingCartItems.Remove(cartItem);
                    }

                    await _unitOfWork.CompleteAsync();

                    var updatedUser = await _unitOfWork.Users.GetByEmailAsync(userEmail);
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
