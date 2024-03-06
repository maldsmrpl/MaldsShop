using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime AddedTime { get; set; }
    public DateTime? LastActivityTime { get; set; }
    public int? ShippingCartId { get; set; }
    public virtual ShippingCart? ShippingCart { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<Review>? Reviews { get; set; }
    public AppUser()
    {
        Orders = new HashSet<Order>();
        Reviews = new HashSet<Review>();
    }
}
