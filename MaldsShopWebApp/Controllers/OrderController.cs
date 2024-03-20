using CloudinaryDotNet;
using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;

namespace MaldsShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IUserRepository _userRepository;

        public OrderController(IOptions<StripeSettings> stripeSettings, IUserRepository userRepository)
        {
            _stripeSettings = stripeSettings.Value;
            _userRepository = userRepository;
        }
        [Authorize]
        public async Task<IActionResult> Index(OrderViewModel orderVM)
        {
            var user = await _userRepository.GetByEmailAsync(orderVM.UserEmail);
            orderVM.AppUser = user;
            orderVM.ShippingCart = user.ShippingCart;

            return View(orderVM);
        }
        [Authorize]
        public async Task<IActionResult> Checkout(OrderViewModel orderVM)
        {
            StripeConfiguration.ApiKey = _stripeSettings.ApiKey;
            var user = await _userRepository.GetByEmailAsync(orderVM.UserEmail);
            orderVM.AppUser = user;
            orderVM.ShippingCart = user.ShippingCart;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions> { },
                Mode = "payment",
                SuccessUrl = Url.Action("Success", "Order", null, Request.Scheme),
                CancelUrl = Url.Action("Cancel", "Order", null, Request.Scheme),
                CustomerEmail = orderVM.UserEmail,
            };

            foreach (var item in orderVM.ShippingCart.ShippingCartItems)
            {
                SessionLineItemOptions sessionLineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Product.Price,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                            Description = item.Product.Description,
                            Images = new List<string>
                            {
                                item.Product.ImageUrl
                            },
                        },
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionLineItemOptions);
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult PaymentConfirmed()
        {
            return View();
        }
    }
}
