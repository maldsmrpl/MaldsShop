using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class ShippingCart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<ShippingCartItem> ShippingCartItems { get; set; }

        public ShippingCart()
        {
            ShippingCartItems = new HashSet<ShippingCartItem>();
        }
    }
}
