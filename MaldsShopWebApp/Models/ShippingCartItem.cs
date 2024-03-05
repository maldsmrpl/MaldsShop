using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class ShippingCartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ShippingCart")]
        public int ShippingCartId { get; set; }
        public virtual ShippingCart ShippingCart { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
