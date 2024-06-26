﻿using CloudinaryDotNet.Actions;
using MaldsShopWebApp.Data;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaldsShopWebApp.Services;

namespace MaldsShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public ProductController(
            IUnitOfWork unitOfWork,
            IPhotoService photoService
            )
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
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
        [Authorize(Roles = UserRoles.Admin)]
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
                    ImageUrl = result.SecureUrl.ToString(),
                    ItemsSold = 0
                };
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();
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
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");
            var detailsVM = new DetailsProductViewModel()
            {
                isAdmin = await _unitOfWork.Users.IsAdminByEmailAsync(User.Identity.Name),
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                InStock = product.InStock,
                ImageUrl = product.ImageUrl,
                Reviews = product.Reviews,
                ItemsSold= product.ItemsSold
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
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return RedirectToAction("Index");
            var editVM = new EditProductViewModel()
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
                if (product.InStock == null) editVM.InStock = 0;
                return View(editVM);
            }
            else
            {
                Console.WriteLine("Product not found");
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id, EditProductViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit product");
                return View("Edit", editVM);
            }

            var oldProduct = await _unitOfWork.Products.GetByIdAsync(id);

            editVM.Reviews = oldProduct.Reviews;
            editVM.ProductId = id;

            if (oldProduct == null) return View("Error");

            var photoResult = new ImageUploadResult();

            if (editVM.Image != null)
            {
                photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                await _photoService.DeletePhotoAsync(oldProduct.ImageUrl);
                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Photo upload failed");
                    return View(editVM);
                }
            }

            var product = new Product
            {
                Title = editVM.Title,
                Description = editVM.Description,
                InStock = editVM.InStock,
                Price = editVM.Price,
                ProductId = id,
                Reviews = editVM.Reviews
            };

            if (photoResult.Bytes > 0)
            {
                product.ImageUrl = photoResult.Url.ToString();
            }
            else
            {
                product.ImageUrl = editVM.ImageUrl;
            }

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productDetails = await _unitOfWork.Products.GetByIdAsync(id);

            if (productDetails == null)
            {
                return View("Error");
            }

            await _unitOfWork.Products.DeleteAsync(productDetails);
            return RedirectToAction("Index", "Home");
        }
    }
}
