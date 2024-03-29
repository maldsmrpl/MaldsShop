﻿using MaldsShopWebApp.Models;

public class Product : SoftDeleteEntity
{
    public int ProductId { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public int? InStock { get; set; }
    public int ItemsSold { get; set; }
    public int Price { get; set; }
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
