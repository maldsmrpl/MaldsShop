using System.Runtime.CompilerServices;

namespace MaldsShopWebApp.ViewModels
{
    public class CreateProductViewModel
    {
        public string AppUserEmail { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IFormFile ImageUrl { get; set; }
        public int? InStock { get; set; }
    }
}
