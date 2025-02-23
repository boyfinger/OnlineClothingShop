using System;
using System.Collections.Generic;

namespace OnlineClothing.Models;

public partial class UserGender
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Userinfo> Userinfos { get; set; } = new List<Userinfo>();
}
