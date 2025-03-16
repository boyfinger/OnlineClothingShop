using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models;

public partial class Product
{
    public long Id { get; set; }

    public Guid? SellerId { get; set; }

    [Display(Name = "Product name")]
    [Required(ErrorMessage = "Please enter product name")]
    public string? Name { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Thumbnail")]
    public string? ThumbnailUrl { get; set; }

    [Display(Name = "Description")]
    [Required(ErrorMessage = "Please enter description")]
    public string? Description { get; set; }

    [Display(Name = "Price")]
    [Required(ErrorMessage = "Please enter price")]
    [Range(minimum: 1, maximum: int.MaxValue,ErrorMessage = "Please enter price as a positive integer")]
    public int? Price { get; set; }

    public int? Discount { get; set; }

    [Display(Name = "Quantity")]
    [Required(ErrorMessage = "Please enter quantity")]
    [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Please enter quantity as a positive integer")]
    public int? Quantity { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string Currency = "VND";

    public virtual Category? Category { get; set; }
    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User? Seller { get; set; }

    public virtual ProductStatus? StatusNavigation { get; set; }

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
