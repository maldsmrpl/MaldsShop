using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime AddedTime { get; set; }
        public DateTime? EditedTime { get; set; }
        public string ReviewText { get; set; }

        [Range(1, 5, ErrorMessage = "Error - Out of range. Expected 1 to 5")]
        public int ReviewScore { get; set; }
    }
}
