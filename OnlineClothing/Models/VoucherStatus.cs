using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class VoucherStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
