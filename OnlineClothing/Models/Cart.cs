using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Cart
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public int? TotalAmount { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual User? User { get; set; }
}
