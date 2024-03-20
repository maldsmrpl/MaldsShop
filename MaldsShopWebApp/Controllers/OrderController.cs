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
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOptions<StripeSettings> stripeSettings, IUserRepository userRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _stripeSettings = stripeSettings.Value;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }
        [Authorize]
        public async Task<IActionResult> Index(OrderViewModel orderVM)
        {
            var user = await _userRepository.GetByEmailAsync(orderVM.UserEmail);
            orderVM.AppUser = user;
            orderVM.ShippingCart = user.ShippingCart;
            foreach (var item in user.ShippingCart.ShippingCartItems)
            {
                if (!await _productRepository.IsEnoughStock(item.Product, item.Quantity))
                {
                    orderVM.IsEnoughStock = false;
                    break;
                }
                orderVM.IsEnoughStock = true;
            }
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

            Order order = new Order()
            {
                AppUserId = user.Id,
                IsPaid = false,
                PurchasedTime = DateTime.UtcNow,
                StripeInvoiceId = session.InvoiceId,
                StripeSessionId = session.Id,
                OrderItems = new List<OrderItem>() { }
            };

            foreach (var item in user.ShippingCart.ShippingCartItems)
            {
                order.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
            }

            await _orderRepository.AddAsync(order);

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
