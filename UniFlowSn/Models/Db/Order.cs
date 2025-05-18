using System;
using System.Collections.Generic;

namespace UniFlowSn.Models.Db;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public DateTime CreateDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
