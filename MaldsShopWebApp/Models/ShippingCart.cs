using System.ComponentModel.DataAnnotations;

public class ShippingCart
{
    [Key]
    public int ShippingCartId { get; set; }
    public string AppUserId { get; set; }
    public virtual AppUser AppUser { get; set; }
    public virtual ICollection<ShippingCartItem> ShippingCartItems { get; set; } = new List<ShippingCartItem>();
}
