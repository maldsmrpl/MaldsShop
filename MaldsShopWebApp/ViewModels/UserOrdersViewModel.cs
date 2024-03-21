namespace MaldsShopWebApp.ViewModels
{
    public class UserOrdersViewModel
    {
        public string UserEmail { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
