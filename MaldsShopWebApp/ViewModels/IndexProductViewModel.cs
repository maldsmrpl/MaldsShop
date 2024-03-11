namespace MaldsShopWebApp.ViewModels
{
    public class IndexProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SortBy { get; set; }
        public string AppUserEmail { get; set; }
        public ShippingCart ShippingCart { get; set; }
    }
}
