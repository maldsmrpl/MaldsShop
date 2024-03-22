using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.ViewModels
{
    public class UserIndexViewModel
    {
        [Required(ErrorMessage = "First name required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last name required")]
        public string? LastName { get; set; }
        public string UserEmail { get; set; }
        public AppUser AppUser { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
    }
}
