using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MaldsShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CartCountSession _cartCountSession;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(
            ILogger<HomeController> logger, 
            CartCountSession cartCountSession,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _cartCountSession = cartCountSession;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string sortBy = "Name")
        {
            var result = await _unitOfWork.Products.GetAllPaginatedAsync(page, pageSize, sortBy);
            if (User?.Identity?.IsAuthenticated == true)
            {
                
                var user = await _unitOfWork.Users.GetByEmailAsync(User.Identity.Name);

                var viewModel = new IndexProductViewModel
                {
                    ShippingCart = user.ShippingCart,
                    Products = result.Items,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    SortBy = sortBy
                };

                var cartItems = user.ShippingCart.ShippingCartItems;
                var itemsInTheCart = 0;
                foreach (var item in cartItems)
                {
                    itemsInTheCart = itemsInTheCart + item.Quantity;
                }

                HttpContext.Session.SetInt32("CartCount", itemsInTheCart);

                return View(viewModel);
            }
            else
            {
                var viewModel = new IndexProductViewModel
                {
                    ShippingCart = new ShippingCart(),
                    Products = result.Items,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    SortBy = sortBy
                };

                return View(viewModel);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}