using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MaldsShopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public UserController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
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
        public IActionResult Reviews()
        {
            return View();
        }
    }
}
