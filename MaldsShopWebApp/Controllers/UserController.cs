using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MaldsShopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;

        public UserController(IOrderRepository orderRepository, IReviewRepository reviewRepository)
        {
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Orders()
        {
            var userEmail = User?.Identity?.Name;

            if (User.Identity.IsAuthenticated && userEmail != null)
            {
                var orders = await _orderRepository.GetOrdersByUserEmailAsync(userEmail);
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
                var reviews = await _reviewRepository.GetAllByUserEmailAsync(userEmail);
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
