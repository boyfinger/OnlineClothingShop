using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class VoucherUsage
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public long VoucherId { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;
}
