using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime AddedTime { get; set; }
    public DateTime? LastActivityTime { get; set; }
    [ForeignKey("ShippingCart")]
    public int ShippingCartId { get; set; }
    public virtual ShippingCart ShippingCart { get; set; } = new ShippingCart();
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<Review>? Reviews { get; set; }
}
