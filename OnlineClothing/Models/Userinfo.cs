using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class Userinfo
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }

    public int? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual UserGender? GenderNavigation { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
