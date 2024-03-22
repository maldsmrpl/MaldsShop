using System.ComponentModel.DataAnnotations;

public class Review
{
    [Key]
    public int ReviewId { get; set; }
    public AppUser AppUser { get; set; }
    public string AppUserId { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public string ReviewText { get; set; }
    [Range(0, 5, ErrorMessage = "Error - Out of range")]
    public int ReviewScore { get; set; }
}
