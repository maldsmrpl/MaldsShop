using System.ComponentModel.DataAnnotations;

public class ShippingCartItem
{
    [Key]
    public int Id { get; set; }
    public int ShippingCartId { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int Quantity { get; set; }
}
