using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public DateTime? RegisterDate { get; set; }

    public int? RecoveryCode { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
