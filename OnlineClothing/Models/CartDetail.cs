using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class CartDetail
{
    public Guid CartId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public int TotalPrice { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
