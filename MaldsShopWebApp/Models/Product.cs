using MaldsShopWebApp.Models;

public class Product : SoftDeleteEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? InStock { get; set; }
    public int Price { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public Product()
    {
        Reviews = new HashSet<Review>();
    }
}
