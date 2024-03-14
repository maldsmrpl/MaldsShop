using Microsoft.AspNetCore.Mvc;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using MaldsShopWebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace MaldsShopWebApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public ReviewController(IReviewRepository reviewRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index(int id)
        {
            var reviews = await _reviewRepository.GetAllByProductId(id);

            var reviewViewModels = reviews.Select(review => new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                AddedTime = review.AddedTime,
                EditedTime = review.EditedTime,
                ReviewText = review.ReviewText,
                ReviewScore = review.ReviewScore,
                ProductId = review.ProductId,
                // Assuming UserEmail is part of your Review model or you have a method to fetch it.
            }).ToList();

            return View(reviewViewModels);
        }
        [Authorize]
        public IActionResult Create(int id)
        {
            CreateReviewViewModel reviewVM = new CreateReviewViewModel()
            {
                ProductId = id
            };
            return View(reviewVM);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewViewModel reviewViewModel)
        {
            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    AppUserId = ( await _userRepository.GetByEmail(User.Identity.Name)).Id,
                    ProductId = reviewViewModel.ProductId,
                    ReviewText = reviewViewModel.ReviewText,
                    ReviewScore = reviewViewModel.ReviewScore,
                    AddedTime = DateTime.UtcNow
                };

                await _reviewRepository.AddAsync(review);
                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }
            return View(reviewViewModel);
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var review = await _reviewRepository.GetById(id);
            if (review == null)
            {
                return NotFound();
            }

            var viewModel = new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                AddedTime = review.AddedTime,
                EditedTime = review.EditedTime,
                ReviewText = review.ReviewText,
                ReviewScore = review.ReviewScore,
                ProductId = review.ProductId,
            };

            return View(viewModel);
        }
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _reviewRepository.GetById(id);
            if (review == null)
            {
                return NotFound();
            }

            var viewModel = new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                ReviewScore = review.ReviewScore,
                ProductId = review.ProductId
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewViewModel reviewViewModel)
        {
            if (id != reviewViewModel.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int productId = 0;
                try
                {
                    var review = await _reviewRepository.GetById(id);
                    review.ReviewText = reviewViewModel.ReviewText;
                    review.ReviewScore = reviewViewModel.ReviewScore;
                    review.EditedTime = DateTime.UtcNow;

                    await _reviewRepository.UpdateAsync(review);
                    productId = review.ProductId;
                }
                catch
                {
                    if (!await ReviewExists(reviewViewModel.ReviewId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (productId == 0) return RedirectToAction("Index", "Home");
                return RedirectToAction("Details", "Product", new { id = productId });
            }
            return View(reviewViewModel);
        }
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _reviewRepository.GetById(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reviewRepository.DeleteAsync(id);

            var review = await _reviewRepository.GetById(id);
            return RedirectToAction(nameof(Index), new { productId = review.ProductId });
        }

        private async Task<bool> ReviewExists(int id)
        {
            var review = await _reviewRepository.GetById(id);
            return review != null;
        }
    }
}
