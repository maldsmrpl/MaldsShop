using System.ComponentModel.DataAnnotations;

public class Review
{
    [Key]
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public DateTime AddedTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public string ReviewText { get; set; }
    [Range(1, 5, ErrorMessage = "Error - Out of range")]
    public int ReviewScore { get; set; }
}
