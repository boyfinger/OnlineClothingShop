using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SellerId { get; set; }

    public long? VoucherId { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Note { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Status { get; set; }

    public int? TotalAmount { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual User? Customer { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? Seller { get; set; }

    public virtual OrderStatus? StatusNavigation { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
