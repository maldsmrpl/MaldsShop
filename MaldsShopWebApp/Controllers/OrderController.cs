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
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(
            IOptions<StripeSettings> stripeSettings,
            IUnitOfWork unitOfWork
            )
        {
            _stripeSettings = stripeSettings.Value;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Details(int id)
        {
            var orderDetailsVM = new OrderDetailsViewModel();
            var email = User?.Identity?.Name;
            if (email != null)
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user != null)
                {
                    var order = await _unitOfWork.Orders.GetOrderByIdAsync(id);
                    if (order != null)
                    {
                        if (user.Id == order.AppUserId)
                        {
                            orderDetailsVM.IsUserVerified = true;
                            orderDetailsVM.Order = order;
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View(orderDetailsVM);
        }
        [Authorize]
        public async Task<IActionResult> Confirmation(OrderViewModel orderVM)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(orderVM.UserEmail);
            orderVM.AppUser = user;
            orderVM.ShippingCart = user.ShippingCart;
            foreach (var item in user.ShippingCart.ShippingCartItems)
            {
                if (!await _unitOfWork.Products.IsEnoughStock(item.Product, item.Quantity))
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
            var user = await _unitOfWork.Users.GetByEmailAsync(orderVM.UserEmail);
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
                        Currency = "uah",
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

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return Redirect(session.Url);
        }
        public async Task<IActionResult> Success()
        {
            var cart = await _unitOfWork.ShippingCarts.GetShippingCartByUserEmail(User.Identity.Name);
            if (cart != null)
            {
                int itemsInTheCart = 0;
                foreach (var item in cart.ShippingCartItems)
                {
                    itemsInTheCart += item.Quantity;
                }

                HttpContext.Session.SetInt32("CartCount", itemsInTheCart);
            }
            
            return View();
        }
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
