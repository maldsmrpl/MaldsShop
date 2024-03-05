using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class Customer : AppUser
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [ForeignKey("ShippingCart")]
        public int? ShippingCartId { get; set; }
        public virtual ShippingCart? ShippingCart { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

        public Customer()
        {
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
        }
    }
}
