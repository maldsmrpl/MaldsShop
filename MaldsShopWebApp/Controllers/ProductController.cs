using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Services;

namespace MaldsShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;

        public ProductController(IProductRepository productRepository, IUserRepository userRepository, IPhotoService photoService)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
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
                if (!_userRepository.IsAdminAsync(productVM.AppUserEmail).Result) return RedirectToAction("Index");

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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(productVM);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");
            var detailsVM = new DetailsProductViewModel()
            {
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                InStock = product.InStock,
                ImageUrl = product.ImageUrl,
                Reviews = product.Reviews
            };
            
            if (product != null)
            {
                if (product.InStock == null) detailsVM.InStock = 0;
                return View(detailsVM);
            }
            else
            {
                Console.WriteLine("Product not found");
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, string appUserEmail)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");
            var editVM = new EditProductViewModel()
            {
                AppUserEmail = appUserEmail,
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                InStock = product.InStock,
                ImageUrl = product.ImageUrl,
                Reviews = product.Reviews
            };

            if (product != null)
            {
                if (product.InStock == null) editVM.InStock = 0;
                return View(editVM);
            }
            else
            {
                Console.WriteLine("Product not found");
                return RedirectToAction("Index");
            }
        }
    }
}
