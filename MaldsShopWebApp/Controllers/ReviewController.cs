using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index(int productId)
        {
            var reviews = await _reviewRepository.GetAllByProductId(productId);

            var reviewViewModels = reviews.Select(review => new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                AddedTime = review.AddedTime,
                EditedTime = review.EditedTime,
                ReviewText = review.ReviewText,
                ReviewScore = review.ReviewScore,
                ProductId = review.ProductId,
            }).ToList();

            return View(reviewViewModels);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel reviewViewModel)
        {
            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    AppUserId = _userRepository.GetByEmail(User.Identity.Name).Result.Id, // This is synchronous; consider async properly.
                    ProductId = reviewViewModel.ProductId,
                    ReviewText = reviewViewModel.ReviewText,
                    ReviewScore = reviewViewModel.ReviewScore,
                    AddedTime = DateTime.Now
                };

                await _reviewRepository.AddAsync(review);
                return RedirectToAction(nameof(Index), new { productId = review.ProductId });
            }
            return View(reviewViewModel);
        }
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
                // UserEmail = Assuming you have a method to fetch this
            };

            return View(viewModel);
        }
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
                ProductId = review.ProductId,
                // Other fields as needed
            };

            return View(viewModel);
        }

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
                try
                {
                    var review = await _reviewRepository.GetById(id);
                    review.ReviewText = reviewViewModel.ReviewText;
                    review.ReviewScore = reviewViewModel.ReviewScore;
                    review.EditedTime = DateTime.Now;

                    await _reviewRepository.UpdateAsync(review);
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
                return RedirectToAction(nameof(Index), new { productId = reviewViewModel.ProductId });
            }
            return View(reviewViewModel);
        }

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
            // Assuming review has a ProductId to redirect back to the reviews list
            var review = await _reviewRepository.GetById(id); // Fetch again to get ProductId for redirect
            return RedirectToAction(nameof(Index), new { productId = review.ProductId });
        }

        private async Task<bool> ReviewExists(int id)
        {
            var review = await _reviewRepository.GetById(id);
            return review != null;
        }
    }
}
