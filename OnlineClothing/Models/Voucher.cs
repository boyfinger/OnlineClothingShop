using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Voucher
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public int? Type { get; set; }

    public decimal? Value { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsageCount { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual VoucherStatus? StatusNavigation { get; set; }

    public virtual VoucherType? TypeNavigation { get; set; }

    public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
}
