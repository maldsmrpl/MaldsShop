namespace MaldsShopWebApp.ViewModels
{
    public class UserReviewViewModel
    {
        public string UserEmail { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
