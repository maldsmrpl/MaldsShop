namespace MaldsShopWebApp.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order? Order { get; set; }
        public bool IsUserVerified { get; set; } = false;
    }
}
