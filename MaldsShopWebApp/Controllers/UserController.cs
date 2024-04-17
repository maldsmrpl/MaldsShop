using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaldsShopWebApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var userEmail = User?.Identity?.Name;

            if (User.Identity.IsAuthenticated && userEmail != null)
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
                var orders = await _unitOfWork.Orders.GetOrdersByUserEmailAsync(userEmail);
                var reviews = await _unitOfWork.Reviews.GetAllByUserEmailAsync(userEmail);

                UserIndexViewModel userVM = new UserIndexViewModel()
                {
                    UserEmail = userEmail,
                    AppUser = user,
                    Orders = orders.Take(5),
                    Reviews = reviews.Take(5),
                };
                return View(userVM);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserIndexViewModel userVM)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(userVM.UserEmail);
            if (user != null)
            {
                user.FirstName = userVM.FirstName;
                user.LastName = userVM.LastName;
                _unitOfWork.Users.Update(user);
                _unitOfWork.CompleteAsync();
            }
            return RedirectToAction("Index", "User");
        }
        public async Task<IActionResult> Orders()
        {
            var userEmail = User?.Identity?.Name;

            if (User.Identity.IsAuthenticated && userEmail != null)
            {
                var orders = await _unitOfWork.Orders.GetOrdersByUserEmailAsync(userEmail);
                UserOrdersViewModel ordersVM = new UserOrdersViewModel()
                {
                    UserEmail = userEmail,
                    Orders = orders
                };
                return View(ordersVM);
            }
            return View();
        }
        public async Task<IActionResult> Reviews()
        {
            var userEmail = User?.Identity?.Name;

            if (User.Identity.IsAuthenticated && userEmail != null)
            {
                var reviews = await _unitOfWork.Reviews.GetAllByUserEmailAsync(userEmail);
                UserReviewViewModel reviewVM = new UserReviewViewModel()
                {
                    UserEmail = userEmail,
                    Reviews = reviews
                };
                return View(reviewVM);
            }
            return View();
        }
    }
}
