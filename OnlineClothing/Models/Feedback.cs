using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public long? ProductId { get; set; }

    public long? OrderId { get; set; }

    public byte? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User User { get; set; } = null!;
}
