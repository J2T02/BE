using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class Account
{
    public int AccId { get; set; }

    public int? RoleId { get; set; }

    public string? AccName { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Role? Role { get; set; }
}
