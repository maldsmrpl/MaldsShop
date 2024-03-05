using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int? InStock { get; set; }
        public int Price { get; set; }

        [ForeignKey("Merchant")]
        public int MerchantId { get; set; }
        public virtual Merchant Merchant { get; set; }
        public int? ItemsSold { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public Product()
        {
            Reviews = new HashSet<Review>();
        }
    }
}
