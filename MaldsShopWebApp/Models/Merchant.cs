using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class Merchant : AppUser
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        public string CompanyName { get; set; }

        public virtual ICollection<Product>? Products { get; set; }

        public Merchant()
        {
            Products = new HashSet<Product>();
        }
    }
}
