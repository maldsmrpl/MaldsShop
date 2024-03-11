using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MaldsShopWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IShippingCartRepository _shippingCartRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, IShippingCartRepository shippingCartRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _shippingCartRepository = shippingCartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, string sortBy = "Name")
        {
            var result = await _productRepository.GetAllPaginatedAsync(page, pageSize, sortBy);

            var viewModel = new IndexProductViewModel
            {
                ShippingCart = await _shippingCartRepository.GetShippingCartByUserEmail(User.Identity.Name),
                Products = result.Items,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                SortBy = sortBy
            };

            return View(viewModel);
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
