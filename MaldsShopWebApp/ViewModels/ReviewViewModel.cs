namespace MaldsShopWebApp.ViewModels
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public string? UserEmail { get; set; }
        public DateTime? AddedTime { get; set; }
        public DateTime? EditedTime { get; set; }
        public string ReviewText { get; set; }
        public int ReviewScore { get; set; }
        public int ProductId { get; set; }
    }

}
