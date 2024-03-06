using System.ComponentModel.DataAnnotations;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public string AppUserId { get; set; }
    public virtual AppUser AppUser { get; set; }
    public DateTime PurchasedTime { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}
