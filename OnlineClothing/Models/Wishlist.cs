using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Wishlist
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long ProductId { get; set; }

    public byte? IsDeleted { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
