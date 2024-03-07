using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Services;

namespace MaldsShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IPhotoService _photoService;

        public ProductController(IProductRepository productRepository, IPhotoService photoService)
        {
            _productRepository = productRepository;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            IndexProductViewModel productsVM = new IndexProductViewModel()
            {
                Products = products
            };
            return View(productsVM);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(productVM.ImageUrl);

                var product = new Product
                {
                    Title = productVM.Title,
                    Description = productVM.Description,
                    Price = productVM.Price,
                    InStock = productVM.InStock,
                    ImageUrl = result.Url.ToString()
                };
                _productRepository.Add(product);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(productVM);
        }
    }
}
