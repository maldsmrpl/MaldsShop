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
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int id)
        {
            var reviews = await _unitOfWork.Reviews.GetAllByProductIdAsync(id);

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
                    AppUserId = ( await _unitOfWork.Users.GetByEmailAsync(User.Identity.Name)).Id,
                    ProductId = reviewViewModel.ProductId,
                    ReviewText = reviewViewModel.ReviewText,
                    ReviewScore = reviewViewModel.ReviewScore,
                    AddedTime = DateTime.UtcNow
                };

                await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Details", "Product", new { id = review.ProductId });
            }
            return View(reviewViewModel);
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
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
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
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
                    var review = await _unitOfWork.Reviews.GetByIdAsync(id);
                    review.ReviewText = reviewViewModel.ReviewText;
                    review.ReviewScore = reviewViewModel.ReviewScore;
                    review.EditedTime = DateTime.UtcNow;

                    await _unitOfWork.Reviews.UpdateAsync(review);
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
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
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
            await _unitOfWork.Reviews.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return RedirectToAction(nameof(Index), new { productId = review.ProductId });
        }

        private async Task<bool> ReviewExists(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return review != null;
        }
    }
}
