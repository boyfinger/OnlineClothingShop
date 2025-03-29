using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class ProductRejectionLog
{
    public int Id { get; set; }

    public long ProductId { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime? RejectedAt { get; set; }

    public int? ResendCount { get; set; }

    public virtual Product Product { get; set; } = null!;
}
