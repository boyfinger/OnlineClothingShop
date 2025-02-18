using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Product
{
    public long Id { get; set; }

    public long? SellerId { get; set; }

    public string? Name { get; set; }

    public long? CategoryId { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? Description { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    public int? Quantity { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User? Seller { get; set; }

    public virtual ProductStatus? StatusNavigation { get; set; }

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
