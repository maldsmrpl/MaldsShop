namespace MaldsShopWebApp.ViewModels
{
    public class OrderViewModel
    {
        public string UserEmail { get; set; }
        public AppUser? AppUser { get; set; }
        public ShippingCart? ShippingCart { get; set; }
        public bool IsEnoughStock { get; set; }
    }
}
