using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required]
        public string ReviewText { get; set; }
        [Required]
        public int ReviewScore { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
