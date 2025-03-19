using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class OrderdetailStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
