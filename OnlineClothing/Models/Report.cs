using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineClothing.Models;

public partial class Report
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public long? ProductId { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [MinLength(10, ErrorMessage = "Reason must be at least 10 characters long.")]
    public string? Reason { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual ReportStatus? StatusNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
