using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class UserVoucher
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public long? VoucherId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Status { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Voucher? Voucher { get; set; }
}
